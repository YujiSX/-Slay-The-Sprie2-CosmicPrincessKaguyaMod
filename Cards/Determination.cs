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
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards;

[Pool(typeof(EventCardPool))]
public sealed class Determination : CustomCardModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Ancient;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public Determination() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    // 数值变量：能量1，抽牌1
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new EnergyVar(1),
        new CardsVar(1)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        base.EnergyHoverTip
    };

    // 固有、保留、消耗
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Innate,
        CardKeyword.Retain,
        CardKeyword.Exhaust
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得 1 点能量
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, Owner);
        // 抽 1 张牌
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, Owner);
        // 施加 1 层“决心”（下一张攻击牌伤害翻倍）
        await PowerCmd.Apply<DeterminationPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级后能量+1，抽牌+1
        base.DynamicVars.Energy.UpgradeValueBy(1m);
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(Determination)}.png";
}
