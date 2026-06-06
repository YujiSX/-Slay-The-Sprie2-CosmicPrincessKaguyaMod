using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class RoyalTwinBlades : CustomRelicModel
    {
        private HashSet<CardType> _playedTypesThisCombat = new();

        public override RelicRarity Rarity => RelicRarity.Rare;

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new PowerVar<WeakPower>(1m),
            new PowerVar<VulnerablePower>(1m),
            new CardsVar(2)
        };

        public override string PackedIconPath => "res://images/relics/shuangdao.png";
        protected override string PackedIconOutlinePath => "res://images/relics/shuangdao.png";
        protected override string BigIconPath => "res://images/relics/shuangdao.png";

        private HashSet<CardType> PlayedTypesThisCombat
        {
            get => _playedTypesThisCombat;
            set
            {
                AssertMutable();
                _playedTypesThisCombat = value;
            }
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != Owner) return;

            var type = cardPlay.Card.Type;
            if (!PlayedTypesThisCombat.Add(type)) return;

            Flash();

            switch (type)
            {
                case CardType.Attack:
                    var enemies = Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature)
                        .Where(c => c.IsAlive);
                    await PowerCmd.Apply<WeakPower>(
                        choiceContext, enemies, 1, Owner.Creature, null);
                    break;

                case CardType.Skill:
                    var foes = Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature)
                        .Where(c => c.IsAlive);
                    await PowerCmd.Apply<VulnerablePower>(
                        choiceContext, foes, 1, Owner.Creature, null);
                    break;

                case CardType.Power:
                    await CardPileCmd.Draw(choiceContext, 2, Owner);
                    break;
            }
        }

        public override Task AfterCombatEnd(CombatRoom _)
        {
            PlayedTypesThisCombat = new HashSet<CardType>();
            return Task.CompletedTask;
        }
    }
}