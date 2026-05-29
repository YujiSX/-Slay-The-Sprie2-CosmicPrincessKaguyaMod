using Godot;
using Kaguya.HinaMods.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Kaguya.HinaMods;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.HinaMods.Cards;

/// <summary>
/// 月潮回流
/// 1费 技能牌 | 自身目标
/// 消耗2层月夜，获得4层月夜。
/// 升级：获得的月夜变为5层。
/// </summary>
public sealed class MoonTideReturn : HinaModsCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<FortunePower>(4m)
    };
    protected override IEnumerable<IHoverTip> GetCustomHoverTips()
    {
        return new IHoverTip[]
        {
            HoverTipFactory.FromPower<FortunePower>()
        };
    }
    public MoonTideReturn()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        if (cardPlay.IsFirstInSeries)
        {
            var moonPower = Owner.Creature.GetPower<FortunePower>();
            if (moonPower != null && moonPower.Amount >= 2m)
            {
                // 修复：添加 choiceContext 参数
                await PowerCmd.ModifyAmount(
                    choiceContext,
                    moonPower,
                    -2m,
                    Owner.Creature,
                    this);
            }
        }

        // 修复：添加 choiceContext 参数
        await PowerCmd.Apply<FortunePower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(FortunePower)].BaseValue,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(FortunePower)].UpgradeValueBy(1m);
    }

    protected override bool IsPlayable
    {
        get
        {
            if (!base.IsPlayable) return false;
            var moon = Owner.Creature.GetPower<FortunePower>();
            return moon != null && moon.Amount >= 2m;
        }
    }

    protected override bool ShouldGlowGoldInternal => IsPlayable;
}