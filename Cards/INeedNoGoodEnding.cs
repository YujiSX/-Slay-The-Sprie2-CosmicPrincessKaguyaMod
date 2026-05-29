using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class INeedNoGoodEnding : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    // 未升级时具有消耗关键词
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => System.Array.Empty<DynamicVar>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<Overwork>(),
        HoverTipFactory.FromKeyword(RealityKeyword.Reality)   // 添加现实提示
    };

    public INeedNoGoodEnding() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 进入现实
        await PowerCmd.Apply<Reality>(choiceContext, Owner.Creature, 1, Owner.Creature, this);

        // 获取过劳层数
        int overworkAmount = 0;
        var overworkPower = Owner.Creature.GetPower<Overwork>();
        if (overworkPower != null)
        {
            overworkAmount = (int)overworkPower.Amount;
        }

        if (overworkAmount > 0)
        {
            await CardPileCmd.Draw(choiceContext, overworkAmount, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后移除消耗关键词
        RemoveKeyword(CardKeyword.Exhaust);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(INeedNoGoodEnding)}.png";
}
