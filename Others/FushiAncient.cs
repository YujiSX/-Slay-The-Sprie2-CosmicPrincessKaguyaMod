using Godot;
using HarmonyLib;
using Kaguya.Relics;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Events;
using System.Collections.Generic;
using System.Linq;

namespace Kaguya
{
    public sealed class FushiAncient : AncientEventModel
    {
        public override Color ButtonColor => new Color(0.2f, 0.5f, 0.3f, 0.6f);
        public override Color DialogueColor => new Color("2E6B3E");

        // 选项1的池子：两个遗物随机一个
        private List<EventOption> OptionPool1 => new List<EventOption>
        {
            RelicOption<SeaSlugMemory>(),
            RelicOption<SeaSlugBlessing>()
        };

        // 选项2：跨越时空的羁绊（固定）
        private EventOption Option2 => RelicOption<TimelessBond>();

        // 选项3：超级扭蛋机（固定）
        private EventOption Option3 => RelicOption<SuperGashaponMachine>();

        // 所有可能出现的选项（用于图鉴和随机池）
        public override IEnumerable<EventOption> AllPossibleOptions =>
            OptionPool1.Concat(new[] { Option2, Option3 });

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
            // 选项1：从两个遗物中随机一个
            var option1 = Rng.NextItem(OptionPool1);
            // 选项2：固定
            var option2 = Option2;
            // 选项3：固定
            var option3 = Option3;
            return new List<EventOption> { option1, option2, option3 };
        }
    }
}