using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class MamiOnigiri : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient; // 稀有度可根据需要调整

        // 动态变量用于本地化描述（可选）
        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new DynamicVar("HealPerFive", 2m)
        };

        // 图标路径（请根据实际资源调整）
        public override string PackedIconPath => "res://images/relics/mami_onigiri.png";
        protected override string PackedIconOutlinePath => "res://images/relics/mami_onigiri.png";
        protected override string BigIconPath => "res://images/relics/mami_onigiri.png";

        // 战斗结束时触发
        public override async Task AfterCombatEnd(CombatRoom _)
        {
            // 获取牌组中的卡牌数量
            int deckSize = PileType.Deck.GetPile(Owner).Cards.Count;
            int groupsOfFive = deckSize / 6; // 每5张为一组
            if (groupsOfFive <= 0) return;

            int healAmount = groupsOfFive * 2; // 每组回复3点生命
            await CreatureCmd.Heal(Owner.Creature, healAmount);
            Flash(); // 可选，视觉反馈
        }
    }
}
