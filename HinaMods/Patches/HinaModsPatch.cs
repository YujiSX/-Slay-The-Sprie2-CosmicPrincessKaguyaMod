using static Godot.Node;
using Godot;
using HarmonyLib;
using Kaguya.HinaMods.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Kaguya.HinaMods.Character;

[HarmonyPatch(typeof(NCreature), "SetAnimationTrigger")]
public static class HinaModsAnimationPatch
{
	private const string FINAL_ANIM_PREFIX = "Moons";

	public static void Postfix(NCreature __instance, string trigger)
	{
		if (__instance.Entity == null || !__instance.Entity.IsPlayer)
			return;

		if (__instance.Entity.ModelId.ToString() == "CHARACTER.KAGUYA-HINA_CHARACTER")
		{
			bool isFinalForm = __instance.Entity.HasPower<TsukimiBlessingPower>();
			PlayAnimation(__instance, trigger, isFinalForm);
		}
	}

	private static void PlayAnimation(NCreature node, string trigger, bool isFinalForm)
	{
		switch (trigger)
		{
			case "Hit":
				PlayAnim(node, "Hit", false, isFinalForm);
				break;
			case "Attack":
				PlayAnim(node, "Attack", false, isFinalForm);
				break;
			case "Cast":
				PlayAnim(node, "Attack", true, isFinalForm);
				break;
			case "Dead":
				PlayAnim(node, "Dead", false, isFinalForm);
				break;
			default:
				PlayAnim(node, "Idle", false, isFinalForm);
				break;
		}
	}

	// 寮哄埗鍒锋柊褰㈡€佸姩鐢伙紙鏍稿績锛氱珛鍗冲垏鎹㈠綋鍓嶅舰鎬侊級
	public static void RefreshForm(Creature creature)
	{
		if (!creature.IsPlayer || creature.ModelId.ToString() != "CHARACTER.KAGUYA-HINA_CHARACTER")
			return;

		if (NCombatRoom.Instance == null)
			return;

		NCreature creatureNode = NCombatRoom.Instance.GetCreatureNode(creature);
		if (creatureNode == null)
			return;

		bool isFinalForm = creature.HasPower<TsukimiBlessingPower>();
		PlayAnim(creatureNode, "Idle", false, isFinalForm);
	}

	// 搴曞眰鍔ㄧ敾鎾斁锛堟棤淇敼锛岀ǔ瀹氳繍琛岋級
	private static void PlayAnim(NCreature node, string animName, bool fromEnd, bool isFinalForm)
	{
		var visual = node.GetNodeOrNull<Node2D>("HinaMods");
		if (visual == null)
			return;

		var anim = visual.GetNodeOrNull<AnimatedSprite2D>("Visuals");
		if (anim == null)
			return;

		string targetAnim = isFinalForm ? $"{FINAL_ANIM_PREFIX}{animName}" : animName;
		string idleAnim = isFinalForm ? $"{FINAL_ANIM_PREFIX}Idle" : "Idle";

		if (anim.IsConnected("animation_finished", Callable.From(() => { })))
			anim.Disconnect("animation_finished", Callable.From(() => { }));

		anim.Frame = 0;
		anim.Play(targetAnim, 1f, fromEnd);

		anim.Connect("animation_finished", Callable.From(() =>
		{
			anim.Play(idleAnim);
		}), 4);
	}
}

// ===================== 褰㈡€佸垏鎹㈢洃鍚ˉ涓?=====================
// 1. 鑾峰緱BUFF 鈫?鍒囨崲鏈€缁堝舰鎬?[HarmonyPatch(typeof(Creature), nameof(Creature.ApplyPowerInternal))]
public static class Creature_ApplyPower_Patch
{
	public static void Postfix(Creature __instance, PowerModel power)
	{
		if (power is TsukimiBlessingPower)
		{
			HinaModsAnimationPatch.RefreshForm(__instance);
		}
	}
}

// 2. 绉婚櫎BUFF 鈫?鍒囧洖鏅€氬舰鎬侊紙浣犺姹傛仮澶嶇殑鍔熻兘锛?[HarmonyPatch(typeof(Creature), nameof(Creature.RemovePowerInternal))]
public static class Creature_RemovePower_Patch
{
	public static void Postfix(Creature __instance, PowerModel power)
	{
		if (power is TsukimiBlessingPower)
		{
			HinaModsAnimationPatch.RefreshForm(__instance);
		}
	}
}

// 3. 鍥炲悎寮€濮?鈫?寮哄埗鍒锋柊褰㈡€侊紙浣犺姹傜殑寮哄埗鍒锋柊锛?[HarmonyPatch(typeof(Creature), nameof(Creature.AfterTurnStart))]
public static class Creature_AfterTurnStart_Patch
{
	public static void Postfix(Creature __instance)
	{
		HinaModsAnimationPatch.RefreshForm(__instance);
	}
}
// 4. 节点进入场景树 → 立即根据能力状态设置动画
//DISABLED_[HarmonyPatch(typeof(NCreature), "_Ready")]
// 4. AnimatedSprite2D进入场景树 → 根据角色能力状态动态设置autoplay
//DISABLED_[HarmonyPatch(typeof(AnimatedSprite2D), "_Ready")]
public static class AnimatedSprite2D_Ready_Patch
{
	public static void Postfix(AnimatedSprite2D __instance)
	{
		var parent = __instance.GetParent();
		if (parent == null || parent.Name != "HinaMods")
			return;
		var grandparent = parent.GetParent();
		if (grandparent is not NCreature creature)
			return;
		if (creature.Entity == null || !creature.Entity.IsPlayer)
			return;
		if (creature.Entity.ModelId.ToString() != "CHARACTER.KAGUYA-HINA_CHARACTER")
			return;
		if (creature.Entity.HasPower<TsukimiBlessingPower>())
		{
			__instance.Autoplay = "MoonsIdle";
			__instance.Play("MoonsIdle");
		}
	}
}
