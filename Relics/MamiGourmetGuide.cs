using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using System.Linq;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class MamiGourmetGuide : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient; // 稀有度可调整

        // 动态变量用于本地化描述（可选）
        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new DynamicVar("Threshold", 15m) // 每10张牌
        };

        // 图标路径（请根据实际资源调整）
        public override string PackedIconPath => "res://images/relics/mami_gourmet_guide.png";
        protected override string PackedIconOutlinePath => "res://images/relics/mami_gourmet_guide.png";
        protected override string BigIconPath => "res://images/relics/mami_gourmet_guide.png";

        // 修改抽牌数量
        public override decimal ModifyHandDraw(Player player, decimal count)
        {
            if (player != Owner) return count;

            // 获取当前牌组中的卡牌数量
            int deckSize = PileType.Deck.GetPile(Owner).Cards.Count;
            int extraDraws = deckSize / 15; // 每10张多抽1张

            if (extraDraws > 0)
                return count + extraDraws;
            return count;
        }
    }
}