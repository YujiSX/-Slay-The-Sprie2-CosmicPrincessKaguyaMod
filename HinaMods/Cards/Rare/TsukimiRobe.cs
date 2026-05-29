using Kaguya.HinaMods.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Kaguya.HinaMods.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.HinaMods.Cards;

/// <summary>
/// 月见长袍
/// 3费 能力牌 | 自身目标
/// 获得1层月见长袍。
/// 升级：获得2层月见长袍。
/// </summary>
public sealed class TsukimiRobe : HinaModsCard
{
    // 动态变量：基础倍率1，对标群蛇形态规范
    protected override IEnumerable<DynamicVar> CanonicalVars => new[]
    {
        new PowerVar<TsukimiRobeBuff>(1m)
    };
    protected override IEnumerable<IHoverTip> GetCustomHoverTips()
    {
        return new IHoverTip[]
        {
            HoverTipFactory.FromPower<TsukimiRobeBuff>(),
            HoverTipFactory.FromPower<FortunePower>()
        };
    }

    public TsukimiRobe()
        : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Player player = Owner;
        if (player == null) return;

        // 施法动画
        await CreatureCmd.TriggerAnim(player.Creature, "Cast", player.Character.CastAnimDelay);

        // 🔥 修复：补全官方强制参数 choiceContext
        await PowerCmd.Apply<TsukimiRobeBuff>(
            choiceContext,
            player.Creature,
            DynamicVars[nameof(TsukimiRobeBuff)].BaseValue,
            player.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(TsukimiRobeBuff)].UpgradeValueBy(1m);
    }
}