using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Kaguya.Relics
{
    public sealed class MemoryRecallRestSiteOption : RestSiteOption
    {
        private bool _isEnabled;
        public override bool IsEnabled => _isEnabled;
        public override string OptionId => "MEMORY_RECALL";

        public override LocString Description
        {
            get
            {
                if (_isEnabled)
                {
                    return new LocString("rest_site_ui", "OPTION_MEMORY_RECALL.description");
                }
                return new LocString("rest_site_ui", "OPTION_MEMORY_RECALL.descriptionDisabled");
            }
        }

        public MemoryRecallRestSiteOption(Player owner) : base(owner)
        {
            // 仅当玩家拥有任一可升级的记忆碎片时启用
            _isEnabled = owner.Relics.Any(r =>
                r is MemoryFragmentStart ||
                r is MemoryFragmentDaily ||
                r is MemoryFragmentConcert);
        }

        public override async Task<bool> OnSelect()
        {
            if (!_isEnabled) return false;


            // 2. 查找当前持有的记忆碎片（按升级顺序）
            var currentRelic = Owner.Relics.FirstOrDefault(r =>
                r is MemoryFragmentStart ||
                r is MemoryFragmentDaily ||
                r is MemoryFragmentConcert);

            if (currentRelic == null) return false;

            // 3. 确定目标碎片类型并直接创建新遗物
            RelicModel newRelic = null;
            if (currentRelic is MemoryFragmentStart)
                newRelic = ModelDb.Relic<MemoryFragmentDaily>().ToMutable();
            else if (currentRelic is MemoryFragmentDaily)
                newRelic = ModelDb.Relic<MemoryFragmentConcert>().ToMutable();
            else if (currentRelic is MemoryFragmentConcert)
                newRelic = ModelDb.Relic<MemoryFragmentResolution>().ToMutable();

            if (newRelic == null) return false;

            // 4. 移除当前遗物并添加新遗物
            await RelicCmd.Remove(currentRelic);
            await RelicCmd.Obtain(newRelic, Owner);

            return true;
        }
    }
}