using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.Cards; // 引入其他卡牌类型
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class Petition : CustomCardModel
{
    private const int energyCost = 1;      // 升级后变为0
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public Petition() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => System.Array.Empty<DynamicVar>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获取消耗牌堆中的卡牌，并过滤掉排除的卡牌类型
        var exhaustPile = PileType.Exhaust.GetPile(Owner);
        var exhaustCards = exhaustPile.Cards
            .Where(c => !(c is Petition || c is NeverAccept || c is SearchTheWorld))
            .ToList();
        if (exhaustCards.Count == 0) return;

        // 弹出选择界面
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
        var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, exhaustCards, Owner, prefs);
        var card = selected.FirstOrDefault();
        if (card == null) return;

        // 将选中的卡牌加入手牌
        await CardPileCmd.Add(card, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        // 升级：费用减少1（1 → 0）
        EnergyCost.UpgradeBy(-1);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(Petition)}.png";
}