using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class ChallengeLetter : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;

        private const int EnergyPerTurn = 1;
        private const int StrengthGain = 1;

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new EnergyVar(EnergyPerTurn),
            new PowerVar<StrengthPower>("Strength", StrengthGain)
        };

        protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
        {
            HoverTipFactory.ForEnergy(this),
            HoverTipFactory.FromPower<StrengthPower>()
        };

        public override string PackedIconPath => "res://images/relics/challenge_letter.png";
        protected override string PackedIconOutlinePath => "res://images/relics/challenge_letter.png";
        protected override string BigIconPath => "res://images/relics/challenge_letter.png";

        // 每回合+1能量
        public override decimal ModifyMaxEnergy(Player player, decimal amount)
        {
            if (player != Owner) return amount;
            return amount + EnergyPerTurn;
        }

        // 每回合开始时，你和所有敌人获得力量
        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner) return;

            Flash();

            // 给玩家自己加力量
            await PowerCmd.Apply<StrengthPower>(
                new ThrowingPlayerChoiceContext(),
                Owner.Creature,
                StrengthGain,
                null, null);

            var enemies = Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature)
                .Where(c => c.IsAlive);

            // 给所有敌人加力量
            await PowerCmd.Apply<StrengthPower>(
                new ThrowingPlayerChoiceContext(),
                enemies,
                StrengthGain,
                null, null);
        }
    }
}
