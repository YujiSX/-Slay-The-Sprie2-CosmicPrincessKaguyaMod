using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class LiveStreaming : CustomRelicModel
    {
        private int _cardsPlayedThisTurn = 0;
        private bool _nextTurnPenalty = false;

        public override RelicRarity Rarity => RelicRarity.Ancient;

        protected override IEnumerable<DynamicVar> CanonicalVars => new[]
        {
            new DynamicVar("ExtraDraw", 1m),
            new DynamicVar("PenaltyDraw", 1m)
        };

        public override string PackedIconPath => "res://images/relics/live_streaming.png";
        protected override string PackedIconOutlinePath => "res://images/relics/live_streaming_outline.png";
        protected override string BigIconPath => "res://images/relics/live_streaming_big.png";

        public override Task BeforeCombatStart()
        {
            _cardsPlayedThisTurn = 0;
            _nextTurnPenalty = false;
            return Task.CompletedTask;
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != Owner) return;
            if (!CombatManager.Instance.IsInProgress) return;
            _cardsPlayedThisTurn++;
            await Task.CompletedTask;
        }

        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side != Owner.Creature.Side) return;

            _nextTurnPenalty = (_cardsPlayedThisTurn >= 5);
            _cardsPlayedThisTurn = 0;
            await Task.CompletedTask;
        }

        public override decimal ModifyHandDraw(Player player, decimal count)
        {
            if (player != Owner) return count;

            if (_nextTurnPenalty)
            {
                _nextTurnPenalty = false;  // 惩罚仅一次
                return count - 1;          // 惩罚：少抽1张（基础-1）
            }
            return count + 1;              // 正常：多抽1张
        }
    }
}
