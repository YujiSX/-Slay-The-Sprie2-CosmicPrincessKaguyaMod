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
    public sealed class DollSizedGirl : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Event;

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new DynamicVar("GoldCost", 80m)
        };

        protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
        {
            HoverTipFactory.FromKeyword(AdoptKeyword.Adopt)
        };

        public override string PackedIconPath => "res://images/relics/doll_sized_girl.png";
        protected override string PackedIconOutlinePath => "res://images/relics/doll_sized_girl.png";
        protected override string BigIconPath => "res://images/relics/doll_sized_girl.png";

        public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
        {
            if (player != Owner) return false;
            options.Add(new DollSizedGirlRestSiteOption(player, this));
            return true;
        }
    }
}