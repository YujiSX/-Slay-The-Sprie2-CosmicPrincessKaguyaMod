using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya
{
    public static class ScryHelper
    {
        /// <summary>
        /// 执行“预见”操作：查看抽牌堆顶部 amount 张牌，玩家可以选择任意张丢弃（放入弃牌堆）。
        /// </summary>
        /// <param name="player">当前玩家</param>
        /// <param name="amount">要预见的牌数（实际不足则取全部）</param>
        /// <param name="choiceContext">卡牌打出时的上下文（用于选择界面）</param>
        /// <returns>实际被丢弃的卡牌列表</returns>
        public static async Task<List<CardModel>> Scry(Player player, int amount, PlayerChoiceContext choiceContext)
        {
            var drawPile = PileType.Draw.GetPile(player);
            var topCards = drawPile.Cards.Take(amount).ToList();
            if (topCards.Count == 0) return new List<CardModel>();

            // 构造选择界面：玩家可以自由选择任意张（0 到 topCards.Count 张）
            var prefs = new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 0, topCards.Count);
            var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, topCards, player, prefs);
            var toDiscard = selected.ToList();

            // 将选中的牌从抽牌堆移动到弃牌堆
            foreach (var card in toDiscard)
            {
                // Add 会自动从原牌堆（抽牌堆）移除，并添加到目标牌堆
                await CardPileCmd.Add(card, PileType.Discard);
            }

            return toDiscard;
        }
    }
}