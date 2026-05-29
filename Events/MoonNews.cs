using Godot;
using MegaCrit.Sts2.Core.Audio.Debug;
using Kaguya.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Events
{
    public sealed class MoonNews : EventModel
    {
        private int? _musicHandle;

        protected override IEnumerable<DynamicVar> CanonicalVars => new[]
        {
            new HealVar(10)
        };

        // 进入事件时开始播放音乐
        public override void OnRoomEnter()
        {
            base.OnRoomEnter();
            _musicHandle = NDebugAudioManager.Instance.Play("Conisch - News Tsukuyomi!!.ogg");
        }

        protected override IReadOnlyList<EventOption> GenerateInitialOptions()
        {
            return new[]
            {
                new EventOption(this, Listen, InitialOptionKey("LISTEN")),
                new EventOption(this, Reject, InitialOptionKey("REJECT"))
            };
        }

        private async Task Listen()
        {
            var questCard = Owner.RunState.CreateCard<YachiyoCup>(Owner);
            var addResult = await CardPileCmd.Add(questCard, PileType.Deck);
            CardCmd.PreviewCardPileAdd(new[] { addResult }, 2f);
            StopMusic();
            SetEventFinished(L10NLookup("MOON_NEWS.pages.LISTEN.description"));
        }

        private async Task Reject()
        {
            await CreatureCmd.Heal(Owner.Creature, 10, true);
            StopMusic();
            SetEventFinished(L10NLookup("MOON_NEWS.pages.REJECT.description"));
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
