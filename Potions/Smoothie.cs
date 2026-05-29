using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.PotionPools;
using Kaguya.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Potions
{
    [Pool(typeof(KaguyaPotionPool))]  
    public class Smoothie : CustomPotionModel
    {
        public override PotionRarity Rarity => PotionRarity.Rare;
        public override PotionUsage Usage => PotionUsage.CombatOnly;
        public override TargetType TargetType => TargetType.Self;  // 对自身使用

        // 动态变量仅用于描述（可选）
        protected override IEnumerable<DynamicVar> CanonicalVars => System.Array.Empty<DynamicVar>();

        // 悬浮提示：显示“回忆”关键词
        public override IEnumerable<IHoverTip> ExtraHoverTips => new[]
       {
            HoverTipFactory.FromKeyword(MemoryKeyword.Memory)
        };

        // 药水图标（请根据实际资源路径调整）
        public override string CustomPackedImagePath => "res://images/potions/smoothie.png";
        public override string CustomPackedOutlinePath => "res://images/potions/smoothie_outline.png";

        // 使用药水时的逻辑
        protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature target)
        {
            var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);

            // 从抽牌堆选择一张牌
            var drawPile = PileType.Draw.GetPile(Owner);
            var drawCards = drawPile.Cards.ToList();
            if (drawCards.Count > 0)
            {
                var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, drawCards, Owner, prefs);
                var card = selected.FirstOrDefault();
                if (card != null)
                    await ProcessRecall(card, choiceContext);
            }

            // 从弃牌堆选择一张牌
            var discardPile = PileType.Discard.GetPile(Owner);
            var discardCards = discardPile.Cards.ToList();
            if (discardCards.Count > 0)
            {
                var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, discardCards, Owner, prefs);
                var card = selected.FirstOrDefault();
                if (card != null)
                    await ProcessRecall(card, choiceContext);
            }
        }

        private async Task ProcessRecall(CardModel card, PlayerChoiceContext choiceContext)
        {
            // 升级卡牌
            CardCmd.Upgrade(card);
            // 消耗卡牌
            await CardCmd.Exhaust(choiceContext, card);

            // 只有成功升级的卡牌才加入召回列表
            if (card.IsUpgraded)
            {
                var recallPower = Owner.Creature.GetPower<RecallPower>();
                if (recallPower == null)
                {
                    await PowerCmd.Apply<RecallPower>(choiceContext, Owner.Creature, 1, Owner.Creature, null);
                    recallPower = Owner.Creature.GetPower<RecallPower>();
                }
                recallPower?.AddCard(card);
            }
        }
    }
}