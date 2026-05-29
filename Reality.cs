using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Kaguya.Powers
{
    public sealed class Reality : PowerModel
    {
        private bool _hasAppliedOverworkThisTurn = false;

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override PowerInstanceType InstanceType => PowerInstanceType.None;
        public override bool AllowNegative => false;

        // 增加回合开始时的抽牌数量
        public override decimal ModifyHandDraw(Player player, decimal count)
        {
            if (player != Owner.Player) return count;
            return count + 1;
        }

        // 改为在打出牌时触发（每回合第一次打出牌时施加一层过劳）
        public override async Task BeforeCardPlayed(CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner.Creature != Owner) return;
            if (_hasAppliedOverworkThisTurn) return;

            _hasAppliedOverworkThisTurn = true;
            Flash();
            var choiceContext = new ThrowingPlayerChoiceContext();
            await PowerCmd.Apply<Overwork>(choiceContext, Owner, 1, Owner, null);
        }

        // 回合结束时重置标志
        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side == Owner.Side)
            {
                _hasAppliedOverworkThisTurn = false;
            }
        }

        // 进入时移除月读
        public override async Task AfterApplied(Creature applier, CardModel cardSource)
        {
            var tsukuyomiPowers = Owner.Powers.OfType<Tsukuyomi>().ToList();
            foreach (var power in tsukuyomiPowers)
                await PowerCmd.Remove(power);
        }

        // 退出时获得 1 能量
        public override async Task AfterRemoved(Creature oldOwner)
        {
            Flash();
            await PlayerCmd.GainEnergy(1, Owner.Player);
        }
    }
}
