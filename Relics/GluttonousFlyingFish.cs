using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class GluttonousFlyingFish : CustomRelicModel
    {
        private bool _usedThisCombat;
        private bool _anyCardsPlayedThisTurn;

        public override RelicRarity Rarity => RelicRarity.Ancient;

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new PowerVar<IntangiblePower>(1)
        };

        protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
        {
            HoverTipFactory.FromPower<IntangiblePower>()
        };

        public override string PackedIconPath => "res://images/relics/gluttonous_flying_fish.png";
        protected override string PackedIconOutlinePath => "res://images/relics/gluttonous_flying_fish_outline.png";
        protected override string BigIconPath => "res://images/relics/gluttonous_flying_fish_big.png";

        public override bool ShowCounter => true;
        public override int DisplayAmount => _usedThisCombat ? 1 : 0;

        private void UpdateDisplay()
        {
            Status = _usedThisCombat ? RelicStatus.Normal : (_anyCardsPlayedThisTurn ? RelicStatus.Normal : RelicStatus.Active);
            InvokeDisplayAmountChanged();
        }

        public override Task BeforeCombatStart()
        {
            _usedThisCombat = false;
            _anyCardsPlayedThisTurn = false;
            UpdateDisplay();
            return Task.CompletedTask;
        }

        public override Task AfterCombatEnd(CombatRoom _)
        {
            _usedThisCombat = false;
            _anyCardsPlayedThisTurn = false;
            UpdateDisplay();
            return Task.CompletedTask;
        }

        // 回合开始时重置本回合打牌标志（使用官方 IsPartOfPlayerTurn 方法）
        public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
        {
            if (side != CombatSide.Player) return Task.CompletedTask;
            if (_usedThisCombat) return Task.CompletedTask;
            if (!CombatManager.Instance.IsPartOfPlayerTurn(Owner)) return Task.CompletedTask;

            _anyCardsPlayedThisTurn = false;
            UpdateDisplay();
            return Task.CompletedTask;
        }

        // 打出牌时记录
        public override Task BeforeCardPlayed(CardPlay cardPlay)
        {
            if (_usedThisCombat) return Task.CompletedTask;
            if (cardPlay.Card.Owner != Owner) return Task.CompletedTask;

            _anyCardsPlayedThisTurn = true;
            UpdateDisplay();
            return Task.CompletedTask;
        }

        // 回合结束前检查是否没有打出任何牌
        public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (_usedThisCombat) return;
            if (_anyCardsPlayedThisTurn) return;
            if (side != CombatSide.Player) return;
            if (!CombatManager.Instance.IsPartOfPlayerTurn(Owner)) return;

            Flash();
            await PowerCmd.Apply<IntangiblePower>(choiceContext, Owner.Creature, 1, Owner.Creature, null);
            _usedThisCombat = true;
            UpdateDisplay();
        }
    }
}
