using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards
{
    public sealed class StarlitSea : CardModel
    {
        // 定义动态变量，用于本地化显示抽牌数差异
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            new[] { new CardsVar(1) };  // 基础抽1张

        public StarlitSea() : base(2, CardType.Power, CardRarity.Ancient, TargetType.Self) { }
        public override IEnumerable<CardTag> Tags => new[] { (CardTag)1001 };

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int amount = IsUpgraded ? 2 : 1;
            await PowerCmd.Apply<StarlitSeaPower>(choiceContext, Owner.Creature, amount, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            // 升级时更新动态变量（抽牌数从1变为2）
            DynamicVars.Cards.UpgradeValueBy(1);
        }
    }
}
