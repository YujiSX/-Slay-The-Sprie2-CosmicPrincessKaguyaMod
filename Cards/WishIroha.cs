using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;  // 引入 PileType
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class WishIroha : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseGold = 20;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new GoldVar(baseGold)
    };
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<Debt>(),
    ];

    // 消耗关键词
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public WishIroha() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得金币
        await PlayerCmd.GainGold(DynamicVars.Gold.BaseValue, Owner);

        var debtCard = base.CombatState.CreateCard<Debt>( base.Owner);
        if (debtCard != null)
        {
            // 将债务卡加入弃牌堆
            await CardPileCmd.AddGeneratedCardToCombat(debtCard, PileType.Discard, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：金币 +10 (20 → 30)
        DynamicVars.Gold.UpgradeValueBy(10);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(WishIroha)}.png";
}