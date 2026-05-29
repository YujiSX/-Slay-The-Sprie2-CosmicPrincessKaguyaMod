using BaseLib.Abstracts;
using Kaguya.CardPools;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Threading.Tasks;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Powers
{
    public sealed class KaguyaNoEnergy : CustomPowerModel
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Single;

        // BaseLib 模板：可选的图标路径（请根据实际资源调整或删除）
        public override string CustomPackedIconPath => "res://images/powers/kaguya_no_energy.png";
        public override string CustomBigIconPath => "res://images/powers/kaguya_no_energy.png";

        public override decimal ModifyEnergyGain(Player player, decimal amount)
        {
            if (player != Owner.Player) return amount;
            Flash();
            return 0;
        }

        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            await PowerCmd.Remove(this);
        }
    }
}
