using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class KaguyaHammer : CustomRelicModel
    {
        private const int AttackThreshold = 6;

        private int _attacksPlayedThisCombat; // 已打出的攻击牌数量（不含当前正在打出的）
        private bool _isActivating;

        public override RelicRarity Rarity => RelicRarity.Ancient;

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new DynamicVar("Threshold", AttackThreshold)
        };

        public override string PackedIconPath => "res://images/relics/kaguya_hammer.png";
        protected override string PackedIconOutlinePath => "res://images/relics/kaguya_hammer_outline.png";
        protected override string BigIconPath => "res://images/relics/kaguya_hammer_big.png";

        public override bool ShowCounter => true;
        public override int DisplayAmount => _attacksPlayedThisCombat % AttackThreshold;

        private void UpdateDisplay()
        {
            if (_isActivating)
                Status = RelicStatus.Normal;
            else
                Status = (_attacksPlayedThisCombat % AttackThreshold == AttackThreshold - 1) ? RelicStatus.Active : RelicStatus.Normal;
            InvokeDisplayAmountChanged();
        }

        // 删除 BeforeCombatStart，计数器不会重置

        public override decimal ModifyDamageMultiplicative(Creature target, decimal amount, ValueProp props, Creature dealer, CardModel cardSource)
        {
            if (dealer != Owner.Creature) return 1m;
            if (cardSource == null) return 1m;
            if (cardSource.Type != CardType.Attack) return 1m;

            if (_attacksPlayedThisCombat % AttackThreshold == AttackThreshold - 1)
            {
                TaskHelper.RunSafely(DoActivateVisuals());
                return 2m;
            }
            return 1m;
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != Owner) return;
            if (cardPlay.Card.Type != CardType.Attack) return;

            _attacksPlayedThisCombat++;
            UpdateDisplay();
            await Task.CompletedTask;
        }

        private async Task DoActivateVisuals()
        {
            if (_isActivating) return;
            _isActivating = true;
            UpdateDisplay();
            Flash();
            await Cmd.Wait(1f);
            _isActivating = false;
            UpdateDisplay();
        }
    }
}