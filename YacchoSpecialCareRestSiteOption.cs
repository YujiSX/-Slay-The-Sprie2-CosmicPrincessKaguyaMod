using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Relics
{
    public sealed class YacchoSpecialCareRestSiteOption : RestSiteOption
    {
        private const int HealAmount = 6;

        public override string OptionId => "YACCHO_SPECIAL_CARE";

        public override LocString Description
        {
            get
            {
                var loc = new LocString("rest_site_ui", $"OPTION_{OptionId}.description");
                if (IsEnabled)
                {
                    loc.Add("Heal", HealAmount);
                    return loc;
                }
                return new LocString("rest_site_ui", $"OPTION_{OptionId}.descriptionDisabled");
            }
        }

        public YacchoSpecialCareRestSiteOption(Player owner) : base(owner)
        {
            IsEnabled = true; // 可根据需求修改条件
        }

        public override async Task<bool> OnSelect()
        {
            if (!IsEnabled) return false;

            // 回复6点生命，第三个参数为 bool，表示是否播放动画
            await CreatureCmd.Heal(Owner.Creature, HealAmount, true);

            // 选择一张牌变化
            var prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1);
            var selected = await CardSelectCmd.FromDeckForTransformation(prefs: prefs, player: Owner);
            var cardToTransform = selected.FirstOrDefault();
            if (cardToTransform != null)
            {
                await CardCmd.TransformToRandom(cardToTransform, Owner.RunState.Rng.Niche);
            }

            return true;
        }
    }
}
