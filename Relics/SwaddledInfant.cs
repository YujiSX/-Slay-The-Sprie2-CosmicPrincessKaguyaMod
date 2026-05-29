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
    public sealed class SwaddledInfant : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Event;

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new DynamicVar("GoldCost", 50m)
        };

        protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
        {
            HoverTipFactory.Static(StaticHoverTip.Transform),
            HoverTipFactory.FromKeyword(NurtueKeyword.Nurture)
        };

        public override string PackedIconPath => "res://images/relics/swaddled_infant.png";
        protected override string PackedIconOutlinePath => "res://images/relics/swaddled_infant.png";
        protected override string BigIconPath => "res://images/relics/swaddled_infant.png";

        public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
        {
            if (player != Owner) return false;
            options.Add(new NurtureRestSiteOption(player, this));
            return true;
        }
    }
}