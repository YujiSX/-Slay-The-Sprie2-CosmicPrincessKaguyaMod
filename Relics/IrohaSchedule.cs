using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.Powers;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
    public sealed class IrohaSchedule : CustomRelicModel
    {
        private bool _hasAppliedOverworkThisTurn = false;

        public override RelicRarity Rarity => RelicRarity.Ancient;

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new EnergyVar(1),
            new PowerVar<Overwork>(1)
        };

        protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
        {
            HoverTipFactory.FromPower<Overwork>()
        };

        public override string PackedIconPath => "res://images/relics/iroha_schedule.png";
        protected override string PackedIconOutlinePath => "res://images/relics/iroha_schedule_outline.png";
        protected override string BigIconPath => "res://images/relics/iroha_schedule_big.png";

        public override Task BeforeCombatStart()
        {
            _hasAppliedOverworkThisTurn = false;
            return Task.CompletedTask;
        }

        // 每回合开始时获得1点能量（新版钩子）
        public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
        {
            if (side != CombatSide.Player) return;
            var player = Owner;
            if (player == null) return;
            await PlayerCmd.GainEnergy(1, player);
        }

        // 修正：BeforeCardPlayed 只接收 CardPlay 参数（无 PlayerChoiceContext）
        public override async Task BeforeCardPlayed(CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != Owner) return;
            if (_hasAppliedOverworkThisTurn) return;

            _hasAppliedOverworkThisTurn = true;
            Flash();

            // PowerCmd.Apply 需要 PlayerChoiceContext，这里使用占位上下文
            await PowerCmd.Apply<Overwork>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1, Owner.Creature, null);
        }

        // 回合结束时重置标志
        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side != Owner.Creature.Side) return;
            _hasAppliedOverworkThisTurn = false;
            await Task.CompletedTask;
        }
    }
}
