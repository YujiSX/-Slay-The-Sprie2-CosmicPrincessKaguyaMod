using Godot;
using HarmonyLib;
using Kaguya;
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
    public sealed class RokaAndMamiAncient : AncientEventModel
    {
        public override Color ButtonColor => new Color(0.9f, 0.5f, 0.2f, 0.6f);
        public override Color DialogueColor => new Color("4A2F1D");

        // 选项池1：三个遗物中随机一个
        private List<EventOption> OptionPool1 => new List<EventOption>
        {
            RelicOption<RokaNailArt>(),
            RelicOption<RokaMakeupBag>(),
            RelicOption<CharmingAntler>()
        };

        // 选项池2：三个遗物中随机一个
        private List<EventOption> OptionPool2 => new List<EventOption>
        {
            RelicOption<MamiGourmetGuide>(),
            RelicOption<MamiOnigiri>(),
            RelicOption<SpoonAndFork>()
        };

        // 选项池3：固定一个遗物
        private List<EventOption> OptionPool3 => new List<EventOption>
        {
            RelicOption<GashaponMachineL3>()
        };

        public override IEnumerable<EventOption> AllPossibleOptions =>
            OptionPool1.Concat(OptionPool2).Concat(OptionPool3);

        protected override AncientDialogueSet DefineDialogues()
        {
            return new AncientDialogueSet
            {
                FirstVisitEverDialogue = new AncientDialogue("", "", "") { VisitIndex = 0 },
                CharacterDialogues = new Dictionary<string, IReadOnlyList<AncientDialogue>>(),
                AgnosticDialogues = new AncientDialogue[] { new AncientDialogue("") }
            };
        }

        protected override IReadOnlyList<EventOption> GenerateInitialOptions()
        {
            var option1 = Rng.NextItem(OptionPool1);
            var option2 = Rng.NextItem(OptionPool2);
            var option3 = Rng.NextItem(OptionPool3);
            return new List<EventOption> { option1, option2, option3 };
        }
    }
}
