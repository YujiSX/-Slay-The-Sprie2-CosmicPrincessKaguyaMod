using Godot;
using HarmonyLib;
using Kaguya.PotionPools;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using System.Collections.Generic;
using System.Linq;

namespace Kaguya.PotionPools
{
    public sealed class KaguyaPotionPool : PotionPoolModel
    {
        // 能量颜色名称（与卡池/遗物池保持一致）
        public override string EnergyColorName => "kaguya";

        // 药水描边着色颜色（在图鉴等界面显示）
        public override Color LabOutlineColor => new Color("#D4AF37"); // 金色

        // 返回该药水池包含的所有药水
        protected override List<PotionModel> GenerateAllPotions()
        {
            return new List<PotionModel>
            {
                // 示例：占位符药水（请替换为实际的自定义药水类）
                // ModelDb.Potion<MyCustomPotion>(),
                // ModelDb.Potion<AnotherPotion>(),
            };
        }
    }
}
[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.AllPotionPools), MethodType.Getter)]
public static class ModelDbAllPotionPoolsPatch
{
    static void Postfix(ref IEnumerable<PotionPoolModel> __result)
    {
        __result = __result
            .Append(ModelDb.PotionPool<KaguyaPotionPool>())
            .Distinct();
    }
}