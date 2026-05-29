using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
public class WorkRelentlessly : CustomCardModel
{
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int creationMultiplier = 2;   // 每点能量获得2层创作
    private const int overworkAmount = 2;       // 固定2层过劳

    // X 费
    protected override bool HasEnergyCostX => true;

    // 动态变量：用于本地化描述显示倍数和过劳
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("CreationMultiplier", creationMultiplier),
        new PowerVar<Overwork>(overworkAmount)
    };

    // 未升级时具有消耗关键词
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<CreationPower>(),
        HoverTipFactory.FromPower<Overwork>()
    };

    public WorkRelentlessly() : base(0, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int xValue = ResolveEnergyXValue();
        if (xValue <= 0) return;

        // 获得 2X 层创作
        int creationAmount = creationMultiplier * xValue;
        await PowerCmd.Apply<CreationPower>(choiceContext, Owner.Creature, creationAmount, Owner.Creature, this);

        // 获得 2 层过劳
        await PowerCmd.Apply<Overwork>(choiceContext, Owner.Creature, overworkAmount, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级后移除消耗关键词
        RemoveKeyword(CardKeyword.Exhaust);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(WorkRelentlessly)}.png";
}
