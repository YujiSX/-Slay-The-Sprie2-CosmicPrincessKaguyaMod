using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya
{
    public sealed class Reply : CardModel
    {
        // 基础版消耗，升级后添加保留
        public override IEnumerable<CardKeyword> CanonicalKeywords =>
            IsUpgraded ? new[] { CardKeyword.Retain } : new[] { CardKeyword.Exhaust };
        public override IEnumerable<CardTag> Tags => new[] { (CardTag)1001 };

        // 动态变量（仅用于本地化显示能量图标）
        protected override IEnumerable<DynamicVar> CanonicalVars => new[]
        {
            new EnergyVar(1) // 描述用，实际获得能量为变量
        };
        protected override bool ShouldGlowGoldInternal => PileType.Exhaust.GetPile(Owner).Cards.Count >= 8;

        public Reply() : base(3, CardType.Skill, CardRarity.Ancient, TargetType.Self) { }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int exhaustCount = PileType.Exhaust.GetPile(Owner).Cards.Count;

            if (exhaustCount >= 8)
            {
                // 分支1：消耗堆牌数 ≥ 8，打出消耗堆中所有牌（排除所有 Reply 卡牌）
                var exhaustPile = PileType.Exhaust.GetPile(Owner);
                // 关键修改：排除所有 Reply 类型的卡牌，不仅仅是当前实例
                var cardsToPlay = exhaustPile.Cards.Where(c => !(c is Reply)).ToList();

                foreach (var card in cardsToPlay)
                {
                    // 自动打出（不消耗能量，目标由游戏内部决定）
                    await CardCmd.AutoPlay(choiceContext, card, null);
                    // 自动打出后，卡牌通常进入弃牌堆；需要将其移回消耗堆（除非已经在消耗堆）
                    if (card.Pile?.Type != PileType.Exhaust)
                    {
                        await CardCmd.Exhaust(choiceContext, card);
                    }
                }
            }
            else
            {
                // 分支2：消耗所有手牌（不包括自身）
                var handCards = PileType.Hand.GetPile(Owner).Cards.ToList();
                var cardsToExhaust = handCards.Where(c => c != this).ToList();
                int exhaustedCount = cardsToExhaust.Count;

                foreach (var card in cardsToExhaust)
                {
                    await CardCmd.Exhaust(choiceContext, card);
                }

                if (exhaustedCount > 0)
                {
                    await PlayerCmd.GainEnergy(exhaustedCount, Owner);
                    await CardPileCmd.Draw(choiceContext, exhaustedCount, Owner);
                }
            }
        }

        protected override void OnUpgrade()
        {
            // 升级后添加保留关键词（消耗已被移除）
            AddKeyword(CardKeyword.Retain);
        }
    }
}