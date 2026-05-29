using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards
{
    public sealed class Satisfied : CardModel
    {
        // 无法升级
        public override int MaxUpgradeLevel => 0;

        // 动态变量：获得再生的层数（用于本地化显示）
        protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new PowerVar<RegenPower>(3) };

        // 卡牌关键词：不可打出 + 虚无（回合结束时消耗）
        public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
        {
            CardKeyword.Unplayable,
            CardKeyword.Ethereal
        };

        // 自定义标签，用于区分辉夜姬状态卡
        public override IEnumerable<CardTag> Tags => new[] { (CardTag)1000 };

        // 构造函数：-1费，状态牌，稀有度状态，无目标
        public Satisfied() : base(-1, CardType.Status, CardRarity.Status, TargetType.None) { }

        // 抽到这张牌时触发
        public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
        {
            if (card != this) return;

            // 短暂延迟，模拟效果
            await Cmd.Wait(0.25f);

            // 获得2层再生
            await PowerCmd.Apply<RegenPower>(choiceContext, Owner.Creature, 3, Owner.Creature, this);
        }
    }
}
