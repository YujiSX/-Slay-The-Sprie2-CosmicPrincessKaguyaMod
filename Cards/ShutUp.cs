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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class ShutUp : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    private const int baseBlock = 10;
    private const int baseWeak = 2;
    private const int overworkRemove = 2;

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(baseBlock, ValueProp.Move),
        new PowerVar<WeakPower>(baseWeak)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<Overwork>()
    };

    public ShutUp() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得格挡
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        // 给予虚弱
        var target = cardPlay.Target;
        if (target != null)
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, target, DynamicVars.Weak.BaseValue, Owner.Creature, this);
        }

        // 移除2层过劳
        var overwork = Owner.Creature.GetPower<Overwork>();
        if (overwork != null)
        {
            int currentAmount = (int)overwork.Amount;
            int overworkRemove = 2; // 移除层数，请根据实际情况命名变量
            if (currentAmount <= overworkRemove)
            {
                await PowerCmd.Remove(overwork);
            }
            else
            {
                await PowerCmd.Remove(overwork);
                await PowerCmd.Apply<Overwork>(choiceContext, Owner.Creature, currentAmount - overworkRemove, Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：格挡+3，虚弱+1
        DynamicVars.Block.UpgradeValueBy(3);
        DynamicVars.Weak.UpgradeValueBy(1);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(ShutUp)}.png";
}
