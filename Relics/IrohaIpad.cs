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
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class IrohaIpad : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new PowerVar<CreationPower>(2)
        };

        protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
        {
            HoverTipFactory.FromPower<Overwork>(),
            HoverTipFactory.FromPower<CreationPower>()
        };

        public override string PackedIconPath => "res://images/relics/iroha_ipad.png";
        protected override string PackedIconOutlinePath => "res://images/relics/iroha_ipad_outline.png";
        protected override string BigIconPath => "res://images/relics/iroha_ipad_big.png";

        // 使用新版回合开始钩子
        public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
        {
            if (side != CombatSide.Player) return;
            var player = Owner;
            if (player == null) return;

            // 移除1层过劳
            var overwork = player.Creature.GetPower<Overwork>();
            if (overwork != null)
            {
                int currentAmount = (int)overwork.Amount;
                var context = new ThrowingPlayerChoiceContext();
                if (currentAmount <= 1)
                {
                    await PowerCmd.Remove(overwork);
                }
                else
                {
                    await PowerCmd.Remove(overwork);
                    await PowerCmd.Apply<Overwork>(context, player.Creature, currentAmount - 1, player.Creature, null);
                }
            }

            // 获得2层创作
            var choiceContext = new ThrowingPlayerChoiceContext();
            await PowerCmd.Apply<CreationPower>(choiceContext, player.Creature, 2, player.Creature, null);

            Flash();
        }
    }
}
