using BaseLib.Utils;
using Godot;
using Kaguya.HinaMods.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.HinaMods.Cards;

/// <summary>
/// 月蚀
/// 1费 技能牌 | 单体目标
/// 消耗1层月夜，对目标施加1层虚弱，获得7点格挡。
/// 升级：格挡变为10点。
/// </summary>
public sealed class HinaModsMoonEclipse : HinaModsCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<FortunePower>(1m),
        new PowerVar<WeakPower>(1m),
        new BlockVar(7m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> GetCustomHoverTips()
    {
        return new IHoverTip[]
        {
            HoverTipFactory.FromPower<FortunePower>(),
            HoverTipFactory.FromPower<WeakPower>()
        };
    }

    protected override bool IsPlayable
    {
        get
        {
            if (!base.IsPlayable)
                return false;

            decimal requiredMoon = base.DynamicVars["FortunePower"].BaseValue;
            FortunePower fortunePower = base.Owner.Creature.GetPower<FortunePower>();

            return fortunePower != null && fortunePower.Amount >= requiredMoon;
        }
    }

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    public HinaModsMoonEclipse()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal cost = base.DynamicVars["FortunePower"].BaseValue;
        FortunePower fortunePower = base.Owner.Creature.GetPower<FortunePower>();

        if (cardPlay.IsFirstInSeries)
        {
            if (fortunePower != null && fortunePower.Amount >= cost)
            {
                // 修复：添加 choiceContext 参数
                await PowerCmd.ModifyAmount(
                    choiceContext,
                    fortunePower,
                    -cost,
                    base.Owner.Creature,
                    this);
            }
        }

        // 修复：添加 choiceContext 参数
        await PowerCmd.Apply<WeakPower>(
            choiceContext,
            cardPlay.Target,
            base.DynamicVars["WeakPower"].BaseValue,
            base.Owner.Creature,
            this
        );

        await CommonActions.CardBlock(this, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}