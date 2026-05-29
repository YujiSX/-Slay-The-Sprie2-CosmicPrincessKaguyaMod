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
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class PartTimeJob : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Basic;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseEnergy = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new EnergyVar(baseEnergy)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromKeyword(TsukuyomiKeyword.Tsukuyomi),
        HoverTipFactory.FromKeyword(RealityKeyword.Reality),
        HoverTipFactory.FromPower<Overwork>()
    };

    public PartTimeJob() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 检查是否处于“月读”状态
        bool hasTsukuyomi = Owner.Creature.Powers.Any(p => p is Tsukuyomi);

        if (hasTsukuyomi)
        {
            // 若处于月读，则进入现实（施加现实状态，月读会被现实自动移除）
            await PowerCmd.Apply<Reality>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        }
        else
        {
            // 获得能量（数值由动态变量决定，升级后增加）
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
            // 获得1层过劳
            await PowerCmd.Apply<Overwork>(choiceContext, Owner.Creature, 1, Owner.Creature, this);

        }
    }

    protected override void OnUpgrade()
    {
        // 升级后费用减少1（从1变为0）
        EnergyCost.UpgradeBy(-1);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(PartTimeJob)}.png";
}
