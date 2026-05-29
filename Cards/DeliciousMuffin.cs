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
public class DeliciousMuffin : CustomCardModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new EnergyVar(1)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<Overwork>()
    };

    public DeliciousMuffin() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得能量
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);

        // 清除过劳层数
        int clearAmount = IsUpgraded ? 2 : 1;
        var overwork = Owner.Creature.GetPower<Overwork>();
        if (overwork != null)
        {
            int currentAmount = (int)overwork.Amount;
            if (clearAmount >= currentAmount)
            {
                await PowerCmd.Remove(overwork);
            }
            else
            {
                await PowerCmd.Remove(overwork);
                await PowerCmd.Apply<Overwork>(choiceContext, Owner.Creature, currentAmount - clearAmount, Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级清除层数在代码中处理
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(DeliciousMuffin)}.png";
}
