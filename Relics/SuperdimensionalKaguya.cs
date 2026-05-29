using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.Monsters;          // 宠物类
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class SuperdimensionalKaguya : CustomRelicModel
    {
        private int _cardsPlayedThisCombat = 0;
        private bool _hasTriggeredThisCombat = false;
        private int _extraRestOptionUsed; // 0=未用额外选择, 1=已用

        public override RelicRarity Rarity => RelicRarity.Event;

        // 添加宠物支持
        public override bool AddsPet => true;

        // 战斗计数器（进行中且未触发重放时显示）
        public override bool ShowCounter => CombatManager.Instance.IsInProgress && !_hasTriggeredThisCombat;
        public override int DisplayAmount => _cardsPlayedThisCombat;

        public override string PackedIconPath => "res://images/relics/superdimensional_kaguya.png";
        protected override string PackedIconOutlinePath => "res://images/relics/superdimensional_kaguya.png";
        protected override string BigIconPath => "res://images/relics/superdimensional_kaguya.png";

        // 篝火：重置额外选择机会
        public override async Task AfterRoomEntered(AbstractRoom room)
        {
            if (room is RestSiteRoom)
                _extraRestOptionUsed = 0;
            await Task.CompletedTask;
        }

        // 战斗开始：重置计数 + 召唤宠物
        public override async Task BeforeCombatStart()
        {
            _cardsPlayedThisCombat = 0;
            _hasTriggeredThisCombat = false;
            UpdateDisplay();
            await SummonPet();          // 每场战斗召唤宠物
        }

        // 获得遗物时立即召唤（若在战斗中）
        public override async Task AfterObtained()
        {
            if (CombatManager.Instance.IsInProgress)
                await SummonPet();
        }

        // 每张牌计数，第8张触发重放
        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != Owner) return;
            if (_hasTriggeredThisCombat) return;

            _cardsPlayedThisCombat++;
            UpdateDisplay();

            if (_cardsPlayedThisCombat == 8)
            {
                _hasTriggeredThisCombat = true;
                cardPlay.Card.BaseReplayCount++;
                CardCmd.Preview(cardPlay.Card);
                Flash();
                UpdateDisplay();
            }
            await Task.CompletedTask;
        }

        public override Task AfterCombatEnd(CombatRoom _)
        {
            _cardsPlayedThisCombat = 0;
            _hasTriggeredThisCombat = false;
            UpdateDisplay();
            return Task.CompletedTask;
        }

        // 篝火额外选项（最多2个）
        public override bool ShouldDisableRemainingRestSiteOptions(Player player)
        {
            if (player != Owner)
                return true;

            if (_extraRestOptionUsed == 0)
            {
                _extraRestOptionUsed = 1;
                Flash();
                return false; // 允许再选一个
            }
            return true;
        }

        private void UpdateDisplay()
        {
            InvokeDisplayAmountChanged();
        }

        // 召唤 KaguyaPet 的封装
        private async Task SummonPet()
        {
            await PlayerCmd.AddPet<KaguyaPet>(Owner);
        }
    }
}