using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.HoverTips;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class MemoryFragmentStart : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Starter;
        protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
        {
            HoverTipFactory.FromKeyword(MemoryOptionKeyword.Mo)
        };

        public override string PackedIconPath => "res://images/relics/memory_fragment_start.png";
        protected override string PackedIconOutlinePath => "res://images/relics/memory_fragment_start.png";
        protected override string BigIconPath => "res://images/relics/memory_fragment_start.png";

        public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
        {
            if (player != Owner) return false;
            options.Add(new MemoryRecallRestSiteOption(player));
            return true;
        }
    }
}