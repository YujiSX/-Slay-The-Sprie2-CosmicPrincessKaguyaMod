using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Combat;
using BaseLib.Abstracts;
using Godot;
using Kaguya.HinaMods.Cards.Rare;
using Kaguya.HinaMods.SupportCards.Moons;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Kaguya.HinaMods.Cards;
using System.Threading.Tasks;

namespace Kaguya.HinaMods.Powers;

public sealed class TsukimiBlessingPower : CustomPowerModel
{
    // ====================== 基础配置 ======================
    public override PowerType Type => PowerType.Buff;
    // 唯一BUFF，不叠加（对标官方永久增益）
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool AllowNegative => false;
    public override PowerInstanceType InstanceType => PowerInstanceType.None;
    // 图标配置
    public override string CustomPackedIconPath => "res://images/hinamods/Powers/tsukimi_blessing.png";
    public override string CustomBigIconPath => "res://images/hinamods/Powers/tsukimi_blessing.png";
    public override int DisplayAmount => 0;

    // ====================== 【官方原版】获得BUFF：回血5% + 获得升级后的卡牌 ======================
    public override async Task AfterApplied(Creature applier, CardModel cardSource)
    {
        await base.AfterApplied(applier, cardSource);

        // 仅玩家生效
        if (Owner == null || !Owner.IsPlayer || !Owner.IsAlive)
            return;

        // 🔥 完全参考官方休息站公式：最大生命值 * 5%
        decimal healAmount = Owner.MaxHp * 0.05m;

        // 🔥 官方原版回血命令
        await CreatureCmd.Heal(Owner, healAmount);
		// 触发立绘切换为最终形态
		if (NCombatRoom.Instance != null)
		{
			var node = NCombatRoom.Instance.GetCreatureNode(Owner);
			if (node != null)
			{
				var visual = node.GetNodeOrNull<Node2D>("HinaMods");
				if (visual != null)
				{
					var anim = visual.GetNodeOrNull<AnimatedSprite2D>("Visuals");
					if (anim != null)
					{
						anim.Play("MoonsIdle");
					}
				}
			}
		}

    }

    // ====================== 【官方原版】能量上限+1 ======================
    public override decimal ModifyMaxEnergy(Player player, decimal currentEnergy)
    {
        // 仅持有者生效
        if (player != Owner.Player)
            return currentEnergy;

        return currentEnergy + 1;
    }

    // ====================== 【官方原版】每回合抽牌+1 ======================
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        // 仅持有者生效
        if (player != Owner.Player)
            return count;

        return count + 1;
    }
}