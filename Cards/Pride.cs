using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.Cards
{
    public sealed class Pride : CardModel
    {
        // 无法升级
        public override int MaxUpgradeLevel => 0;

        // 卡牌关键词：不可打出
        public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Unplayable };

        // 自定义标签（与辉夜姬状态卡一致）
        public override IEnumerable<CardTag> Tags => new[] { (CardTag)1000 };

        // 构造函数：-1费，状态牌，稀有度状态，无目标
        public Pride() : base(-1, CardType.Status, CardRarity.Status, TargetType.None) { }

        // 抽到这张牌时触发
        public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
        {
            if (card != this) return;

            // 获得15金币
            await PlayerCmd.GainGold(15, Owner);
        }
    }
}