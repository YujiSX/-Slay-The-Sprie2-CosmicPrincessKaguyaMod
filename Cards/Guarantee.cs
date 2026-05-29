using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Cards
{
    [Pool(typeof(CurseCardPool))]
    public sealed class Guarantee : CustomCardModel
    {
        public override int MaxUpgradeLevel => 0;

        public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
        {
            CardKeyword.Unplayable,
            CardKeyword.Eternal
        };

        public override bool HasTurnEndInHandEffect => true;

        public Guarantee() : base(-1, CardType.Curse, CardRarity.Curse, TargetType.None) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => System.Array.Empty<DynamicVar>();

        protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)  // 注意访问修饰符改为 protected
        {
            var hand = PileType.Hand.GetPile(Owner).Cards.Where(c => c != this).ToList();
            if (hand.Count == 0) return;

            var cardToExhaust = Owner.RunState.Rng.CombatCardSelection.NextItem(hand);
            if (cardToExhaust != null)
            {
                await CardCmd.Exhaust(choiceContext, cardToExhaust);
            }
        }

        public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(Guarantee)}.png";
    }
}