using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.Powers;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class FushiDoll : CustomRelicModel
    {
        private bool _hasTriggeredThisCombat = false;

        public override RelicRarity Rarity => RelicRarity.Ancient;

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new PowerVar<Overwork>(3),
            new DynamicVar("ScryAmount", 8)
        };

        protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
        {
            HoverTipFactory.FromPower<Overwork>(),
            HoverTipFactory.FromKeyword(Yujian.yujian)
        };

        public override string PackedIconPath => "res://images/relics/fushi_doll.png";
        protected override string PackedIconOutlinePath => "res://images/relics/fushi_doll_outline.png";
        protected override string BigIconPath => "res://images/relics/fushi_doll_big.png";

        public override Task BeforeCombatStart()
        {
            _hasTriggeredThisCombat = false;
            return Task.CompletedTask;
        }

        // 时机改为 BeforeHandDraw（与 JeweledMask 一致）
        public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
        {
            if (player != Owner) return;
            if (_hasTriggeredThisCombat) return;
            if (combatState.RoundNumber != 1) return; // 仅在首个回合触发

            _hasTriggeredThisCombat = true;
            Flash();

            // 获得3层过劳
            await PowerCmd.Apply<Overwork>(choiceContext, Owner.Creature, 3, Owner.Creature, null);

            // 预见8张牌（现在 choiceContext 是有效的真实上下文）
            await ScryHelper.Scry(Owner, 8, choiceContext);
        }

        public override Task AfterCombatEnd(CombatRoom _)
        {
            _hasTriggeredThisCombat = false;
            return Task.CompletedTask;
        }
    }
}
