using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class Startled : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseWeak = 1;
    private const int baseDraw = 1;
    private const int baseExhaustCount = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<WeakPower>(baseWeak),
        new CardsVar(baseDraw)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<WeakPower>()
    };

    public Startled() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得虚弱
        await PowerCmd.Apply<WeakPower>(choiceContext, Owner.Creature, DynamicVars.Weak.BaseValue, Owner.Creature, this);

        // 从弃牌堆中选择指定数量的牌消耗
        var discardPile = PileType.Discard.GetPile(Owner);
        var discardCards = discardPile.Cards.ToList();
        if (discardCards.Count > 0)
        {
            int exhaustCount = IsUpgraded ? 2 : 1; // 升级后消耗2张
            var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, exhaustCount);
            var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, discardCards, Owner, prefs);
            foreach (var card in selected)
            {
                if (card != null)
                {
                    await CardCmd.Exhaust(choiceContext, card);
                }
            }
        }

        // 抽牌
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        // 升级：抽牌数 +1
        DynamicVars.Cards.UpgradeValueBy(1);
        // 虚弱层数不变，消耗数量通过代码逻辑处理
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(Startled)}.png";
}
