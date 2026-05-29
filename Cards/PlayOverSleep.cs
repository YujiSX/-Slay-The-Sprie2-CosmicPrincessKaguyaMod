using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.Cards
{
    // 加入你的角色卡池
    [Pool(typeof(KaguyaCardPool))]
    public class PlayOverSleep : CustomCardModel
    {
        private const int energyCost = 0;
        private const CardType type = CardType.Skill;
        private const CardRarity rarity = CardRarity.Uncommon;
        private const TargetType targetType = TargetType.Self;
        private const bool shouldShowInCardLibrary = true;

        // 基础数值：获得1点能量，抽1张牌（升级后抽2张）
        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new EnergyVar(1),
            new CardsVar(1)
        };

        // 消耗关键词
        public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<PoorSleep>(),
    ];

        public PlayOverSleep() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 获得1点能量
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);

            // 抽牌（基础值为1，升级后为2）
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);

            // 使用 CombatState.CreateCard 创建原版诅咒牌“睡眠不佳”（PoorSleep）
            var poorSleep = base.CombatState.CreateCard<PoorSleep>(base.Owner);
            if (poorSleep != null)
            {
                // 将诅咒牌加入抽牌堆（PileType.Draw 表示抽牌堆）
                await CardPileCmd.AddGeneratedCardToCombat(poorSleep, PileType.Draw, Owner);
            }
        }

        protected override void OnUpgrade()
        {
            // 升级：抽牌数量增加1（从1变为2）
            DynamicVars.Cards.UpgradeValueBy(1);
        }

        // 可选：卡图路径，modId 请替换为你的实际 modId
        public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(PlayOverSleep)}.png";
    }
}