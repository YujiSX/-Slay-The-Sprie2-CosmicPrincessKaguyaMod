using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Threading.Tasks;

namespace Kaguya.Powers
{
    public sealed class Overwork : PowerModel
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerInstanceType InstanceType => PowerInstanceType.None;
        public override bool AllowNegative => false;

        public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature applier, CardModel cardSource)
        {
            if (power != this) return;
            if (Amount < 6) return;

            int remaining = (int)Amount - 6;
            if (remaining <= 0)
            {
                await PowerCmd.Remove(this);
            }
            else
            {
                await PowerCmd.Remove(this);
                await PowerCmd.Apply<Overwork>(choiceContext, Owner, remaining, null, null);
            }

            await Cmd.Wait(0.01f);

            var player = Owner?.Player;
            if (player != null && Owner.Side == CombatSide.Player)
            {
                if (player.PlayerCombatState.Phase == PlayerTurnPhase.Play)
                {
                    PlayerCmd.EndTurn(player, false);
                }
                else
                {
                    void handler(CombatState _)
                    {
                        CombatManager.Instance.TurnStarted -= handler;
                        PlayerCmd.EndTurn(player, false);
                    }
                    CombatManager.Instance.TurnStarted += handler;
                }
            }
        }
    }
}