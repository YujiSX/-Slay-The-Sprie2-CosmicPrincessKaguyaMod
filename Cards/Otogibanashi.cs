using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Cards
{
    public sealed class Otogibanashi : CardModel
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

        protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new CardsVar(8) };

        public Otogibanashi() : base(3, CardType.Skill, CardRarity.Token, TargetType.Self) { }

        // 条件：抽牌堆为空 且 消耗牌堆牌数 ≥ 8
        private bool CanBeFree => PileType.Draw.GetPile(Owner).Cards.Count == 0 &&
                                   PileType.Exhaust.GetPile(Owner).Cards.Count >= 8;

        // 满足条件时高亮
        protected override bool ShouldGlowGoldInternal => CanBeFree;

        // 重写费用修改方法，满足条件时免费
        public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
        {
            modifiedCost = originalCost;
            if (card != this) return false;
            if (CanBeFree)
            {
                modifiedCost = 0;
                return true;
            }
            return false;
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var handCards = PileType.Hand.GetPile(Owner).Cards.ToList();
            if (handCards.Count == 0) return;

            var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
            var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this);
            var card = selected.FirstOrDefault();
            if (card != null)
            {
                card.BaseReplayCount += 8;
                CardCmd.Preview(card);
            }
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain);
        }
    }
}