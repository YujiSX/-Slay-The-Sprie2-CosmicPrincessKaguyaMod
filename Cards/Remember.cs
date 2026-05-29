using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.GameInfo.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public sealed class Remember : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Ancient;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => new[]
    {
        new EnergyVar(1)
    };

    public override IEnumerable<CardTag> Tags => new[] { (CardTag)1001 };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromKeyword(MemoryKeyword.Memory)
    };

    public Remember() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);

        // 处理抽牌堆
        var drawPile = PileType.Draw.GetPile(Owner);
        var drawCards = drawPile.Cards.ToList();
        if (drawCards.Count > 0)
        {
            var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, drawCards, Owner, prefs);
            var card = selected.FirstOrDefault();
            if (card == null) return;
            await ProcessCard(card, choiceContext);
        }

        // 处理弃牌堆
        var discardPile = PileType.Discard.GetPile(Owner);
        var discardCards = discardPile.Cards.ToList();
        if (discardCards.Count > 0)
        {
            var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, discardCards, Owner, prefs);
            var card = selected.FirstOrDefault();
            if (card == null) return;
            await ProcessCard(card, choiceContext);
        }
    }

    private async Task ProcessCard(CardModel card, PlayerChoiceContext choiceContext)
    {
        // 升级卡牌
        CardCmd.Upgrade(card);
        // 消耗卡牌
        await CardCmd.Exhaust(choiceContext, card);

        // 只有成功升级的卡牌才加入召回列表
        if (card.IsUpgraded)
        {
            var recallPower = Owner.Creature.GetPower<RecallPower>();
            if (recallPower == null)
            {
                await PowerCmd.Apply<RecallPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
                recallPower = Owner.Creature.GetPower<RecallPower>();
            }
            recallPower?.AddCard(card);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/event/{nameof(Remember)}.png";
}
