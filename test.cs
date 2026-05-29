using HarmonyLib;
using Kaguya.Characters;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;
using System;
using System.Reflection;

[HarmonyPatch(typeof(RunManager))]
public static class RichPresencePatch
{
    /// <summary>
    /// 反射获取SteamFriends.SetRichPresence方法
    /// </summary>
    private static readonly Lazy<MethodInfo> SteamSetRichPresence = new(() =>
    {
        var t = AccessTools.TypeByName("Steamworks.SteamFriends");
        return t == null ? null : AccessTools.Method(t, "SetRichPresence", new[] { typeof(string), typeof(string) });
    });
    /// <summary>
    /// 反射获取RunState
    /// </summary>
    private static readonly Lazy<PropertyInfo> StateProp = new(() =>
        AccessTools.DeclaredProperty(typeof(RunManager), "State"));
    /// <summary>
    /// 修改RichPresence
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPostfix]
    [HarmonyPatch("UpdateRichPresence")]
    public static void UpdateRichPresence_Postfix(RunManager __instance)
    {
        if (__instance == null) return;
        // 反射获取State
        var State = StateProp.Value?.GetValue(__instance) as RunState;
        if (!TestMode.IsOn && State != null)
        {
            // 获取玩家
            var player = LocalContext.GetMe(State);
            if (player != null)
            {
                // 获取玩家角色
                var character = player.Character;
                // 只修改特定角色
                switch (character)
                {
                    case SakayoriIroha:
                        // 使用原版的一个角色顶替，否则无法显示
                        SteamSetRichPresence.Value?.Invoke(null, new object[] { "Character", "SILENT" });
                        // 如果你自己做了一个关卡，使用原版的一个关卡顶替，否则无法显示
                        // SteamSetRichPresence.Value?.Invoke(null, new object[] { "Act", "HIVE"});
                        // 这边直接用你想要的名称顶替Ascension
                        SteamSetRichPresence.Value?.Invoke(null, new object[] { "Ascension", " 酒寄彩叶 - A" + State.AscensionLevel });
                        break;
                    
                }
            }
        }
    }
}