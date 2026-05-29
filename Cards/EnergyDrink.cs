using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.Powers; // 包含 Overwork 和 Reality
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
public class EnergyDrink : CustomCardModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseEnergy = 1;
    private const int overworkAmount = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new EnergyVar(baseEnergy),
        new PowerVar<Overwork>(overworkAmount)
    };
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
            HoverTipFactory.FromPower<Overwork>(),
            HoverTipFactory.FromKeyword(RealityKeyword.Reality)
        ];

    // 是否可打出：必须拥有 Reality 能力（层数 > 0）
    protected override bool IsPlayable
    {
        get
        {
            if (!base.IsPlayable) return false;
            var reality = Owner.Creature.GetPower<Reality>();
            return reality != null && reality.Amount > 0;
        }
    }

    // 满足条件时卡牌高亮显示
    protected override bool ShouldGlowGoldInternal => IsPlayable;

    public EnergyDrink() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得能量
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        // 获得过劳能力
        await PowerCmd.Apply<Overwork>(choiceContext, Owner.Creature, 3, Owner.Creature, this);

    }

    protected override void OnUpgrade()
    {
        // 升级：能量 +1（过劳层数不变）
        DynamicVars.Energy.UpgradeValueBy(1);
    }

    // 可选：卡图路径
    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(EnergyDrink)}.png";
}
