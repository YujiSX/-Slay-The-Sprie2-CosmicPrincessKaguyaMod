using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Threading.Tasks;

namespace Kaguya.Enchantments
{
    public sealed class ThoughtEnchantment : CustomEnchantmentModel
    {
        private bool _usedThisTurn;

        public override bool ShowAmount => false;

        protected override string CustomIconPath => "res://images/enchantments/thought_enchantment.png";

        public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(
            CardModel card, bool isAutoPlay, ResourceInfo resources, PileType pileType, CardPilePosition position)
        {
            if (card != Card) return (pileType, position);
            if (_usedThisTurn) return (pileType, position);
            if (pileType == PileType.Discard || pileType == PileType.Exhaust)
            {
                return (PileType.Hand, position);
            }
            return (pileType, position);
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (_usedThisTurn) return;
            if (cardPlay.Card != Card) return;
            _usedThisTurn = true;
        }

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (Card == null) return;
            if (Card.Owner != player) return;
            _usedThisTurn = false;
        }
    }
}