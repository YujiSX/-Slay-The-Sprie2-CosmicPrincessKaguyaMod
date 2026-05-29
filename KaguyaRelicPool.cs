using Godot;
using HarmonyLib;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using System.Collections.Generic;
using System.Linq;

namespace Kaguya.RelicPools
{
    public sealed class KaguyaRelicPool : RelicPoolModel
    {
        // 能量颜色名称（影响遗物相关 UI 的能量色调，与卡池保持一致）
        public override string EnergyColorName => "kaguya";

        // 遗物描边着色颜色（在图鉴等界面显示）
        public override Color LabOutlineColor => new Color("#D4AF37"); // 金色

        // 返回该遗物池包含的所有遗物
        protected override List<RelicModel> GenerateAllRelics()
        {
            return new List<RelicModel>
            {
                
                // 可根据需要继续添加其他辉夜姬相关的遗物
            };
        }
    }
}
[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.AllRelicPools), MethodType.Getter)]
public static class ModelDbAllRelicPoolsPatch
{
    static void Postfix(ref IEnumerable<RelicPoolModel> __result)
    {
        __result = __result
            .Append(ModelDb.RelicPool<KaguyaRelicPool>())
            .Distinct();
    }
}