using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya
{
    public sealed class Melt : CardModel
    {
        // 动态变量：能量值（1），用于本地化显示能量图标
        protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new EnergyVar(1) };
        public override IEnumerable<CardTag> Tags => new[] { (CardTag)1001 };

        // 基础版拥有虚无（Ethereal），升级后保留虚无（不去除）
        public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Ethereal };

        public Melt() : base(2, CardType.Power, CardRarity.Ancient, TargetType.Self) { }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 根据升级状态决定能力层数（1=基础，2=升级）
            int amount = IsUpgraded ? 2 : 1;
            await PowerCmd.Apply<MeltPower>(choiceContext, Owner.Creature, amount, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            // 升级后不移除虚无（保留关键词）
            // 无需额外代码
        }
    }
}
