using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Cards
{
    public sealed class LostSoul : CardModel
    {
        public override int MaxUpgradeLevel => 0;

        // 卡牌关键词：消耗（打出后消耗）
        public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

        // 自定义标签（用于辉夜姬状态卡）
        public override IEnumerable<CardTag> Tags => new[] { (CardTag)1000 };

        public LostSoul() : base(2, CardType.Status, CardRarity.Status, TargetType.None) { }

        public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
        {
            if (card != this) return;

            // 等待一小段时间，让抽牌动画和当前抽牌流程完全结束
            await Task.Delay(500); // 0.5秒

            // 从手牌选择一张牌消耗，但不能选择自己
            var selected = await CardSelectCmd.FromHand(
                choiceContext,
                Owner,
                new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1),
                c => c != this,
                this);
            var toExhaust = selected.FirstOrDefault();
            if (toExhaust != null)
            {
                await CardCmd.Exhaust(choiceContext, toExhaust);
            }
        }
    }
}