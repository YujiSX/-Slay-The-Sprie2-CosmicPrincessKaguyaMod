using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class YacchoSpecialCare : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;

        // 可选：动态变量（用于本地化描述）
        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new DynamicVar("Heal", 6m)
        };

        // 悬浮提示（可自定义）
        protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
        {
            HoverTipFactory.FromKeyword(YacchoKeywords.Yaccho),
        };

        // 图标路径（请根据实际资源调整）
        public override string PackedIconPath => "res://images/relics/yaccho_special_care.png";
        protected override string PackedIconOutlinePath => "res://images/relics/yaccho_special_care_outline.png";
        protected override string BigIconPath => "res://images/relics/yaccho_special_care_big.png";

        // 核心：在休息处添加额外选项
        public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
        {
            if (player != Owner) return false;
            options.Add(new YacchoSpecialCareRestSiteOption(player));
            return true;
        }
    }
}