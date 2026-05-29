using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.GameInfo.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Cards
{
    public sealed class HappySynthesizer : CardModel
    {
        // 基础版无消耗关键词，升级后添加固有
        public override IEnumerable<CardKeyword> CanonicalKeywords =>
            IsUpgraded ? new[] { CardKeyword.Innate } : Enumerable.Empty<CardKeyword>();
        public override IEnumerable<CardTag> Tags => new[] { (CardTag)1001 };

        public HappySynthesizer() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.Self) { }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(EmotionCardKeyword.EmotionCard)
   ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 获取所有状态牌
            var statusCardPool = ModelDb.CardPool<StatusCardPool>();
            var allStatusCards = statusCardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint);

            // 筛选出带有目标标签的卡牌原型
            var eligibleProtos = allStatusCards
                .Where(c => c.Tags.Contains(KaguyaTags.StatusTag))
                .ToList();

            if (eligibleProtos.Count == 0) return;

            var generatedCards = CardFactory.GetDistinctForCombat(
                Owner,
                eligibleProtos,                     // 候选原型列表
                Math.Min(3, eligibleProtos.Count),  // 要选取的数量
                Owner.RunState.Rng.CombatCardGeneration) // 同步 RNG
                .ToList();

            if (generatedCards.Count == 0) return;

            // 弹出选择界面
            var chosenCard = await CardSelectCmd.FromChooseACardScreen(
                choiceContext,
                generatedCards,
                Owner,
                canSkip: true);

            if (chosenCard != null)
            {
                // 将选中的卡牌加入抽牌堆
                await CardPileCmd.AddGeneratedCardToCombat(chosenCard, PileType.Draw, Owner);
            }
        }

        protected override void OnUpgrade()
        {
            // 升级时添加固有关键词（可选，因为 CanonicalKeywords 已经根据 IsUpgraded 返回）
            // 但为了确保所有地方都能识别，调用一次 AddKeyword
            AddKeyword(CardKeyword.Innate);
        }
    }
}