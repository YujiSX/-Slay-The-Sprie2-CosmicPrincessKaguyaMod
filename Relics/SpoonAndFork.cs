using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
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
    public sealed class SpoonAndFork : CustomRelicModel
    {
        private CardType? _lastPlayedCardType = null;
        private int _drawsThisTurn = 0;
        private const int MaxDrawsPerTurn = 2;

        public override RelicRarity Rarity => RelicRarity.Ancient;

        protected override IEnumerable<DynamicVar> CanonicalVars => System.Array.Empty<DynamicVar>();

        public override string PackedIconPath => "res://images/relics/spoon_and_fork.png";
        protected override string PackedIconOutlinePath => "res://images/relics/spoon_and_fork.png";
        protected override string BigIconPath => "res://images/relics/spoon_and_fork.png";

        // 战斗开始时重置记录
        public override Task BeforeCombatStart()
        {
            _lastPlayedCardType = null;
            _drawsThisTurn = 0;
            return Task.CompletedTask;
        }

        // 每回合结束重置每回合抽牌计数器（参考官方 Kusarigama）
        public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side == CombatSide.Player)
            {
                _drawsThisTurn = 0;
            }
            return Task.CompletedTask;
        }

        // 每张牌打出后检测
        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != Owner) return;

            CardType currentType = cardPlay.Card.Type;

            if (_lastPlayedCardType.HasValue && _lastPlayedCardType.Value != currentType && _drawsThisTurn < MaxDrawsPerTurn)
            {
                Flash();
                await CardPileCmd.Draw(context, 1, Owner);
                _drawsThisTurn++;
            }

            _lastPlayedCardType = currentType;
        }
    }
}
