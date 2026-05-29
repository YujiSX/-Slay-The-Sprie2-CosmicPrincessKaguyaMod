using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.PotionPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Potions
{
    [Pool(typeof(KaguyaPotionPool))]  // 请替换为你的药水池
    public class Sushi : CustomPotionModel
    {
        public override PotionRarity Rarity => PotionRarity.Common;
        public override PotionUsage Usage => PotionUsage.CombatOnly;
        public override TargetType TargetType => TargetType.Self;

        // 可选：药水图标（请根据实际资源路径调整）
        public override string CustomPackedImagePath => "res://images/potions/sushi.png";
        public override string CustomPackedOutlinePath => "res://images/potions/sushi_outline.png";

        protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature target)
        {
            // 获取角色卡池中所有伙伴卡（带有标签 (CardTag)1004）
            var allCards = Owner.Character.CardPool.GetUnlockedCards(
                Owner.UnlockState,
                Owner.RunState.CardMultiplayerConstraint);
            var partnerCards = allCards.Where(c => c.Tags.Contains((CardTag)1004)).ToList();

            if (partnerCards.Count == 0) return;

            // 随机取最多3张伙伴卡
            int cardCount = System.Math.Min(3, partnerCards.Count);
            var randomCards = CardFactory.GetDistinctForCombat(Owner, partnerCards, cardCount, Owner.RunState.Rng.CombatCardGeneration).ToList();

            // 让玩家选择一张
            var selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, randomCards, Owner, canSkip: true);
            if (selected == null) return;

            // 创建副本并设置本回合免费
            var generated = selected.CreateClone();
            generated.SetToFreeThisTurn();

            // 加入手牌
            await CardPileCmd.AddGeneratedCardToCombat(generated, PileType.Hand, Owner);
        }
    }
}