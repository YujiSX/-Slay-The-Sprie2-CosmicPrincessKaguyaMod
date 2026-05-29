using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Powers
{
    public sealed class Tsukuyomi : PowerModel
    {
        private bool _isDuplicateRemoval = false; // 标记是否为重复进入时的移除

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override PowerInstanceType InstanceType => PowerInstanceType.None;
        public override bool AllowNegative => false;

        public override async Task AfterApplied(Creature applier, CardModel cardSource)
        {
            // 检查是否已存在另一个月读实例（排除自身）
            var existing = Owner.Powers.OfType<Tsukuyomi>().FirstOrDefault(p => p != this);
            if (existing != null)
            {
                // 重复进入：标记为重复移除，然后移除当前新添加的实例
                _isDuplicateRemoval = true;
                await PowerCmd.Remove(this);
                return;
            }

            // 正常进入月读：移除现实、清除3层过劳、获得1力1敏
            var realityPowers = Owner.Powers.OfType<Reality>().ToList();
            foreach (var power in realityPowers)
                await PowerCmd.Remove(power);

            // 清除3层过劳（不足则全部清除）
            var overwork = Owner.Powers.OfType<Overwork>().FirstOrDefault();
            var choiceContext = new ThrowingPlayerChoiceContext();  // 占位上下文

            if (overwork != null)
            {
                int currentAmount = (int)overwork.Amount;
                if (currentAmount <= 3)
                {
                    await PowerCmd.Remove(overwork);
                }
                else
                {
                    await PowerCmd.Remove(overwork);
                    await PowerCmd.Apply<Overwork>(choiceContext, Owner, currentAmount - 3, Owner, null);
                }
            }

            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, 1, Owner, null);
            await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, 1, Owner, null);
        }

        public override async Task AfterRemoved(Creature oldOwner)
        {
            if (_isDuplicateRemoval) return; // 重复进入导致的移除，不触发退出效果

            Flash();
            var context = new BlockingPlayerChoiceContext();
            await CardPileCmd.Draw(context, 1, Owner.Player);

            var choiceContext = new ThrowingPlayerChoiceContext();
            await PowerCmd.Apply<Overwork>(choiceContext, Owner, 1, Owner, null);
        }
    }
}