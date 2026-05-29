using Godot;
using MegaCrit.Sts2.Core.Audio.Debug;    // 使用 NDebugAudioManager
using Kaguya.Cards;
using Kaguya.Encounters;
using Kaguya.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Events
{
    public sealed class YachiyoCupEvent : EventModel
    {
        private int? _musicHandle;

        public override bool IsShared => true;

        public override bool IsAllowed(IRunState runState) => false;

        // 进入事件房间时播放背景音乐
        public override void OnRoomEnter()
        {
            base.OnRoomEnter();
            if (!_musicHandle.HasValue)
            {
                _musicHandle = NDebugAudioManager.Instance.Play("Conisch - Match Start!!.ogg");
            }
        }

        protected override IReadOnlyList<EventOption> GenerateInitialOptions()
        {
            return new[]
            {
                new EventOption(this, OpponentKaguyaAndIroha, InitialOptionKey("OPPONENT_KAGUYA")),
                new EventOption(this, OpponentBlackOnyx, InitialOptionKey("OPPONENT_BLACK_ONYX")),
                new EventOption(this, GiveUp, InitialOptionKey("GIVE_UP"))
            };
        }

        private async Task OpponentKaguyaAndIroha()
        {
            StopMusic();
            await RemoveQuestCardsForAllPlayers();
            EnterCombatWithoutExitingEvent<IroPEncounter>(Array.Empty<Reward>(), shouldResumeAfterCombat: false);
        }

        private async Task OpponentBlackOnyx()
        {
            StopMusic();
            await RemoveQuestCardsForAllPlayers();

            var rng = Owner.RunState.Rng.Niche;
            var encounterTypes = new[]
            {
                typeof(TeimeiEncounter),
                typeof(ThunderEncounter),
                typeof(NayEncounter)
            };
            var selected = rng.NextItem(encounterTypes);

            if (selected == typeof(TeimeiEncounter))
                EnterCombatWithoutExitingEvent<TeimeiEncounter>(Array.Empty<Reward>(), shouldResumeAfterCombat: false);
            else if (selected == typeof(ThunderEncounter))
                EnterCombatWithoutExitingEvent<ThunderEncounter>(Array.Empty<Reward>(), shouldResumeAfterCombat: false);
            else
                EnterCombatWithoutExitingEvent<NayEncounter>(Array.Empty<Reward>(), shouldResumeAfterCombat: false);
        }

        private async Task GiveUp()
        {
            StopMusic();
            await RemoveQuestCardsForAllPlayers();

            var players = Owner.RunState.Players;
            foreach (var player in players)
            {
                await CreatureCmd.Heal(player.Creature, 15, true);
                await PlayerCmd.GainGold(100, player);
            }
            SetEventFinished(L10NLookup("YACHIYO_CUP_EVENT.pages.GIVE_UP.description"));
        }

        private async Task RemoveQuestCardsForAllPlayers()
        {
            var players = Owner.RunState.Players;
            foreach (var player in players)
            {
                var questCards = player.Deck.Cards.Where(c => c is YachiyoCup).ToList();
                foreach (var card in questCards)
                {
                    PlayerCmd.CompleteQuest(card);
                    await CardPileCmd.RemoveFromDeck(card);
                }
            }
        }

        private void StopMusic()
        {
            if (_musicHandle.HasValue)
            {
                NDebugAudioManager.Instance.Stop(_musicHandle.Value);
                _musicHandle = null;
            }
        }
    }
}
