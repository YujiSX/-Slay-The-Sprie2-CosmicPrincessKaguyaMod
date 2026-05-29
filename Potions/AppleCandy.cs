using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.PotionPools;
using Kaguya.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Potions
{
    [Pool(typeof(KaguyaPotionPool))]  // 请替换为你的药水池
    public class AppleCandy : CustomPotionModel
    {
        public override PotionRarity Rarity => PotionRarity.Common;
        public override PotionUsage Usage => PotionUsage.CombatOnly;
        public override TargetType TargetType => TargetType.Self;

        // 动态变量（可选）
        protected override IEnumerable<DynamicVar> CanonicalVars => System.Array.Empty<DynamicVar>();

        // 悬浮提示：显示月读和现实关键词
        public override IEnumerable<IHoverTip> ExtraHoverTips => new[]
        {
            HoverTipFactory.FromKeyword(TsukuyomiKeyword.Tsukuyomi),
            HoverTipFactory.FromKeyword(RealityKeyword.Reality)
        };

        // 药水图标（请根据实际资源路径调整）
        public override string CustomPackedImagePath => "res://images/potions/apple_candy.png";
        public override string CustomPackedOutlinePath => "res://images/potions/apple_candy_outline.png";

        protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature target)
        {
            // 检查是否处于月读状态
            bool hasTsukuyomi = Owner.Creature.Powers.Any(p => p is Tsukuyomi);

            if (hasTsukuyomi)
            {
                // 进入现实
                await PowerCmd.Apply<Reality>(choiceContext, Owner.Creature, 1, Owner.Creature, null);
            }
            else
            {
                // 进入月读
                await PowerCmd.Apply<Tsukuyomi>(choiceContext, Owner.Creature, 1, Owner.Creature, null);
            }
        }
    }
}