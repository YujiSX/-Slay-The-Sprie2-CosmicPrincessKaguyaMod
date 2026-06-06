using BaseLib.Audio;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using Kaguya;
using Kaguya.Characters;
using Kaguya.Relics;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Random;
using System.Collections.Generic;
using System.Linq;

namespace Kaguya
{
    public sealed class BlackOnyxAncient : AncientEventModel
    {
        public override Color ButtonColor => new Color(0.08f, 0.04f, 0f, 0.75f);
        public override Color DialogueColor => new Color("1A1A1A");

        // 选项池1：DataUsb 或 BrothersDeposit（随机）
        private List<EventOption> OptionPool1 => new List<EventOption>
        {
            RelicOption<DataUsb>(),
            RelicOption<BrothersDeposit>()
        };

        // 选项池2的基础列表（包含 NaiBow）
        private List<EventOption> OptionPool2Base => new List<EventOption>
        {
            RelicOption<NaiBow>(),
            RelicOption<LandmineTrap>(),
            RelicOption<LeiBigSword>(),
            RelicOption<ChallengeLetter>()
        };

        // 选项池2的多人模式列表（排除 NaiBow）
        private List<EventOption> OptionPool2Multiplayer => new List<EventOption>
        {
            RelicOption<LandmineTrap>(),
            RelicOption<LeiBigSword>(),
            RelicOption<ChallengeLetter>()
        };

        // 选项池3：固定为 GashaponMachineL2
        private List<EventOption> OptionPool3 => new List<EventOption>
        {
            RelicOption<GashaponMachineL2>()
        };

        public override IEnumerable<EventOption> AllPossibleOptions =>
            OptionPool1.Concat(OptionPool2Base).Concat(OptionPool3);
        public override void OnRoomEnter()
        {
            base.OnRoomEnter();
            ModAudio.PlaySound("res://audios/blackonxy.ogg", volumeAdd: 15f);
        }

        protected override AncientDialogueSet DefineDialogues()
        {
            return new AncientDialogueSet
            {
                FirstVisitEverDialogue = new AncientDialogue("", "", "") { VisitIndex = 0 },
                CharacterDialogues = new Dictionary<string, IReadOnlyList<AncientDialogue>>
                {
                    [CharKey<Silent>()] = new AncientDialogue[]
                    {
                        new AncientDialogue("", "", "")
                        {
                            VisitIndex = 0
                        }
                    }
                },
                AgnosticDialogues = new AncientDialogue[] { new AncientDialogue("") }
            };
        }

        protected override IReadOnlyList<EventOption> GenerateInitialOptions()
        {
            var option1 = Rng.NextItem(OptionPool1);

            List<EventOption> pool2;
            if (Owner.RunState.Players.Count == 1)
            {
                pool2 = OptionPool2Base;
            }
            else
            {
                pool2 = OptionPool2Multiplayer;
            }
            var option2 = Rng.NextItem(pool2);

            var option3 = Rng.NextItem(OptionPool3);
            return new List<EventOption> { option1, option2, option3 };
        }
    }
}
