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

namespace Kaguya.Cards
{
    public sealed class TransientSymphony : CardModel
    {
        // 动态变量：预见数和保留数（仅用于本地化显示）
        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new DynamicVar("ScryAmount", 3),
            new DynamicVar("MaxRetain", 1)
        };

        public TransientSymphony() : base(1, CardType.Power, CardRarity.Ancient, TargetType.Self) { }
        public override IEnumerable<CardTag> Tags => new[] { (CardTag)1001 };

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 打出时立即进行预见（基础3，升级5）
            int scryAmount = IsUpgraded ? 5 : 3;
            await ScryHelper.Scry(Owner, scryAmount, choiceContext);

            // 施加能力（层数：基础1，升级2）
            int amount = IsUpgraded ? 2 : 1;
            await PowerCmd.Apply<TransientSymphonyPower>(choiceContext, Owner.Creature, amount, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            // 升级时更新动态变量值（仅用于本地化显示）
            DynamicVars["ScryAmount"].UpgradeValueBy(2);  // 3→5
            DynamicVars["MaxRetain"].UpgradeValueBy(1);   // 1→2
        }
    }
}
