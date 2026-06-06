using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Kaguya.Events
{
    public sealed class KaguyaEnding : EventModel
    {
        public override bool IsShared => true;

        public override bool IsAllowed(IRunState runState) => true;

        public override async void OnRoomEnter()
        {
            base.OnRoomEnter();
            await VideoPlayerHelper.PlayFullscreenVideo("res://videoes/Ray.ogv");
        }

        protected override void SetInitialEventState(bool isPreFinished)
        {
            SetEventState(
                L10NLookup("KAGUYA_ENDING.pages.INITIAL.description"),
                GenerateInitialOptions()
            );
        }

        protected override IReadOnlyList<EventOption> GenerateInitialOptions()
        {
            var option = new EventOption(this, Proceed, "PROCEED", false, false);
            option.ThatWontSaveToChoiceHistory();
            return new[] { option };
        }

        private async Task Proceed()
        {
            SetEventFinished(L10NLookup("KAGUYA_ENDING.pages.END.description"));
            await Cmd.Wait(0.5f);
            RunManager.Instance.ActChangeSynchronizer.SetLocalPlayerReady();
        }
    }
}