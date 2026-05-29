using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards
{
    public sealed class Boring : CardModel
    {
        // 消耗关键词
        public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
        public override int MaxUpgradeLevel => 0;

        public Boring() : base(1, CardType.Curse, CardRarity.Curse, TargetType.None) { }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 诅咒牌打出无效果，仅消耗
            await Task.CompletedTask;
        }

        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side != CombatSide.Player) return;

            // 检测是否在弃牌堆
            if (base.Pile?.Type == PileType.Discard)
            {
                var clone = CreateClone();
                if (clone != null)
                {
                    // 将克隆加入弃牌堆（新版签名：card, pileType, creator）
                    var result = await CardPileCmd.AddGeneratedCardToCombat(clone, PileType.Discard, Owner);
                    CardCmd.PreviewCardPileAdd(result, 2.2f);
                }
            }
        }
    }
}
