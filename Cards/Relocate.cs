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
    public sealed class Relocate : CardModel
    {
        // 无法升级
        public override int MaxUpgradeLevel => 0;

        // 卡牌关键词：不可打出
        public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Unplayable };

        // 自定义标签（与辉夜姬状态卡一致）
        public override IEnumerable<CardTag> Tags => new[] { (CardTag)1000 };

        // 构造函数：-1费，状态牌，稀有度状态，无目标
        public Relocate() : base(-1, CardType.Status, CardRarity.Status, TargetType.None) { }

        // 抽到这张牌时触发
        public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
        {
            if (card != this) return;

            // 获取弃牌堆
            var discardPile = PileType.Discard.GetPile(Owner);
            var discardCards = discardPile.Cards.ToList();
            if (discardCards.Count == 0) return;

            // 让玩家从弃牌堆选择一张牌
            var prefs = new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1);
            var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, discardCards, Owner, prefs);
            var selectedCard = selected.FirstOrDefault();
            if (selectedCard == null) return;

            // 将选中的牌移动到抽牌堆顶部
            await CardPileCmd.Add(selectedCard, PileType.Draw, CardPilePosition.Top);
        }
    }
}