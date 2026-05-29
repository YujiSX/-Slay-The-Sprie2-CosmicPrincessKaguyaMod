using BaseLib.Abstracts;
using Godot;
using Kaguya.HinaMods.Powers;
using Kaguya.HinaMods.SupportCards.Common;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Kaguya.HinaMods;
using Kaguya.HinaMods.Cards;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.HinaMods.Cards.Rare;

/// <summary>
/// 支援引导
/// 3费 能力牌 | 自身目标
/// 获得1层支援引导。
/// 升级：费用变为2。
/// </summary>
public sealed class SupportChannelingCard : HinaModsCard
{
    // 构造：3费 能力牌 罕见 目标自身
    public SupportChannelingCard() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self) { }
    protected override IEnumerable<IHoverTip> GetCustomHoverTips()
    {
        return new IHoverTip[]
        {
             HoverTipFactory.FromPower<SupportChannelingPower>(),
             HoverTipFactory.FromCard<SupportStrike>(),
             HoverTipFactory.FromCard<SupportBlock>()
        };
    }

    // 使用卡牌：施加支援引导力量
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Player player = Owner;
        if (player == null) return;

        // 官方施法动画
        await CreatureCmd.TriggerAnim(player.Creature, "Cast", player.Character.CastAnimDelay);
        // 修复：添加 choiceContext 参数
        await PowerCmd.Apply<SupportChannelingPower>(
            choiceContext,
            player.Creature,
            1,
            player.Creature,
            this);
    }

    // 升级：费用-1
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}