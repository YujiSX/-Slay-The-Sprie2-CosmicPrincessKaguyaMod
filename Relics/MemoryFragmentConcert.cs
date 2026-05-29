using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.HoverTips;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class MemoryFragmentConcert : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;
        protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
{
    HoverTipFactory.FromKeyword(MemoryOptionKeyword.Mo)
};

        public override string PackedIconPath => "res://images/relics/memory_fragment_concert.png";
        protected override string PackedIconOutlinePath => "res://images/relics/memory_fragment_concert.png";
        protected override string BigIconPath => "res://images/relics/memory_fragment_concert.png";

        public override async Task AfterObtained()
        {
            NDebugAudioManager.Instance.Play("ForgiveNo.mp3");
            await PlayerCmd.GainGold(100, Owner);
        }

        public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
        {
            if (player != Owner) return false;
            options.Add(new MemoryRecallRestSiteOption(player));
            return true;
        }
    }
}