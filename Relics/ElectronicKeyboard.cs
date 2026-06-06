using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.Powers;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class ElectronicKeyboard : CustomRelicModel
    {
        private const int CreationAmount = 1;

        private HashSet<CardType> _playedTypesThisCombat = new();

        public override RelicRarity Rarity => RelicRarity.Uncommon;

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new PowerVar<CreationPower>(CreationAmount)
        };

        protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
        {
            HoverTipFactory.FromPower<CreationPower>()
        };

        public override string PackedIconPath => "res://images/relics/dianziqing.png";
        protected override string PackedIconOutlinePath => "res://images/relics/dianziqing.png";
        protected override string BigIconPath => "res://images/relics/dianziqing.png";

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
            if (PlayedTypesThisCombat.Add(type))
            {
                Flash();
                await PowerCmd.Apply<CreationPower>(
                    choiceContext,
                    Owner.Creature,
                    CreationAmount,
                    Owner.Creature,
                    null);
            }
        }

        public override Task AfterCombatEnd(CombatRoom _)
        {
            PlayedTypesThisCombat = new HashSet<CardType>();
            return Task.CompletedTask;
        }
    }
}