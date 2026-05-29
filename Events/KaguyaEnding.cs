using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Kaguya.Events
{
    public sealed class KaguyaEnding : EventModel
    {
        private int? _musicHandle;  // 保存音乐句柄

        public override bool IsShared => true;   // 多人合作事件

        public override bool IsAllowed(IRunState runState) => true;

        // 进入事件房间时播放背景音乐（仅播放一次）
        public override void OnRoomEnter()
        {
            base.OnRoomEnter();
            if (!_musicHandle.HasValue)
            {
                _musicHandle = NDebugAudioManager.Instance.Play("Ray.mp3");
            }
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
            // 使用 "PROCEED" 键（显示“继续”）
            var option = new EventOption(this, Proceed, "PROCEED", false, false);
            option.ThatWontSaveToChoiceHistory();
            return new[] { option };
        }

        private async Task Proceed()
        {
            // 停止背景音乐（只停止一次）
            if (_musicHandle.HasValue)
            {
                NDebugAudioManager.Instance.Stop(_musicHandle.Value);
                _musicHandle = null;
            }

            // 显示结束文本
            SetEventFinished(L10NLookup("KAGUYA_ENDING.pages.END.description"));
            await Cmd.Wait(0.5f);
            // 通知同步器当前玩家已准备好（多人模式下每个玩家都会调用）
            RunManager.Instance.ActChangeSynchronizer.SetLocalPlayerReady();
        }
    }
}