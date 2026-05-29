using Godot;
using HarmonyLib;
using Kaguya.Cards;
using Kaguya.Characters; // 假设 SakayoriIroha 在此命名空间
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Events
{
    public sealed class TaketoriMonogatariEvent : EventModel
    {
        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new HpLossVar(6),
            new GoldVar(68)
        };

        public override bool IsAllowed(IRunState runState) => true;

        protected override IReadOnlyList<EventOption> GenerateInitialOptions()
        {
            return new EventOption[]
            {
                new EventOption(this, ReadStory, InitialOptionKey("READ")),
                new EventOption(this, Ignore, InitialOptionKey("IGNORE"))
            };
        }

        // 选项A：阅读故事，获得一张已升级的伙伴牌（仅从 SakayoriIroha 的卡池中选取）
        private async Task ReadStory()
        {
            // 获取 SakayoriIroha 角色实例
            var sakayoriIroha = ModelDb.Character<SakayoriIroha>();
            if (sakayoriIroha == null)
            {
                SetEventFinished(L10NLookup("TAKETORI_MONOGATARI.pages.NO_PARTNER.description"));
                return;
            }

            // 获取该角色卡池中所有解锁的伙伴牌（带有标签 (CardTag)1004）
            var allCards = sakayoriIroha.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint);
            var partnerCards = allCards.Where(c => c.Tags.Contains((CardTag)1004)).ToList();

            if (partnerCards.Count == 0)
            {
                // 如果没有伙伴牌，则给予一张默认卡牌（或直接结束，这里给一个保底）
                SetEventFinished(L10NLookup("TAKETORI_MONOGATARI.pages.NO_PARTNER.description"));
                return;
            }

            // 随机选择一张伙伴牌
            var selected = Owner.RunState.Rng.Niche.NextItem(partnerCards);
            var card = Owner.RunState.CreateCard(selected, Owner);

            // 升级卡牌
            if (card.IsUpgradable)
                CardCmd.Upgrade(card);

            // 加入牌组并播放动画
            var addResult = await CardPileCmd.Add(card, PileType.Deck);
            CardCmd.PreviewCardPileAdd(new[] { addResult }, 2f);

            SetEventFinished(L10NLookup("TAKETORI_MONOGATARI.pages.READ.description"));
        }

        // 选项B：不感兴趣，减少6滴血，获得68金币
        private async Task Ignore()
        {
            // 减少6点生命（不可格挡，不受力量影响）
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, 6,
                ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.SkipHurtAnim, null, null);
            // 获得68金币
            await PlayerCmd.GainGold(68, Owner);
            SetEventFinished(L10NLookup("TAKETORI_MONOGATARI.pages.IGNORE.description"));
        }
    }
}
