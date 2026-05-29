using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.PotionPools;
using Kaguya.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.Potions
{
    [Pool(typeof(KaguyaPotionPool))]
    public class Pancake : CustomPotionModel
    {
        public override PotionRarity Rarity => PotionRarity.Uncommon;
        public override PotionUsage Usage => PotionUsage.CombatOnly;
        public override TargetType TargetType => TargetType.Self;

        // 动态变量：用于显示再生层数
        protected override IEnumerable<DynamicVar> CanonicalVars => new[]
        {
            new PowerVar<RegenPower>(5)  // 5层再生
        };

        // 悬浮提示：显示再生能力
        public override IEnumerable<IHoverTip> ExtraHoverTips => new[]
        {
            HoverTipFactory.FromPower<RegenPower>()
        };

        // 药水图标（请根据实际资源路径调整）
        public override string CustomPackedImagePath => "res://images/potions/pancake.png";
        public override string CustomPackedOutlinePath => "res://images/potions/pancake_outline.png";

        protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature target)
        {
            // 移除所有过劳
            var overwork = Owner.Creature.GetPower<Overwork>();
            if (overwork != null)
            {
                await PowerCmd.Remove(overwork);
            }

            // 获得5层再生
            int regenAmount = (int)DynamicVars["RegenPower"].BaseValue;
            await PowerCmd.Apply<RegenPower>(choiceContext, Owner.Creature, regenAmount, Owner.Creature, null);
        }
    }
}