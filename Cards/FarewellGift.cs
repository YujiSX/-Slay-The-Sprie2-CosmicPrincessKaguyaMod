using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class FarewellGift : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseGoldPerCard = 5;
    private const int upgradedGoldPerCard = 8;

    // 不需要动态变量，因为金币数量是动态计算的
    protected override IEnumerable<DynamicVar> CanonicalVars => System.Array.Empty<DynamicVar>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public FarewellGift() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获取消耗牌堆的卡牌数量
        var exhaustPile = PileType.Exhaust.GetPile(Owner);
        int exhaustCount = exhaustPile.Cards.Count;

        int goldPerCard = IsUpgraded ? upgradedGoldPerCard : baseGoldPerCard;
        int totalGold = exhaustCount * goldPerCard;

        if (totalGold > 0)
        {
            await PlayerCmd.GainGold(totalGold, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果在 OnPlay 中通过 IsUpgraded 处理，无需额外代码
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(FarewellGift)}.png";
}