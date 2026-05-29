using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards
{
    public sealed class Ray : CardModel
    {
        // 自身消耗
        public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
        public override IEnumerable<CardTag> Tags => new[] { (CardTag)1001 };

        // 高亮提示：当手牌数量（不含自身） ≥ 8 时发光
        protected override bool ShouldGlowGoldInternal
        {
            get
            {
                var handCards = PileType.Hand.GetPile(Owner).Cards;
                int count = handCards.Count(c => c != this);
                return count >= 8;
            }
        }

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
            new[] { HoverTipFactory.FromKeyword(CardKeyword.Exhaust) };

        public Ray() : base(2, CardType.Skill, CardRarity.Ancient, TargetType.Self) { }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

            // 获取当前手牌（排除自身）
            var handCards = PileType.Hand.GetPile(Owner).Cards.Where(c => c != this).ToList();
            int exhaustedCount = handCards.Count;

            if (exhaustedCount == 0) return;

            // 消耗所有手牌
            foreach (var card in handCards)
            {
                await CardCmd.Exhaust(choiceContext, card);
            }

            // 抽取等量的牌
            await CardPileCmd.Draw(choiceContext, exhaustedCount, Owner);

            // 若消耗牌数 ≥ 8，新抽的牌本回合免费打出
            if (exhaustedCount >= 8)
            {
                var newHand = PileType.Hand.GetPile(Owner).Cards.ToList();
                foreach (var card in newHand)
                {
                    if (!card.EnergyCost.CostsX)
                        card.SetToFreeThisTurn();
                }
            }
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain);
        }
    }
}
