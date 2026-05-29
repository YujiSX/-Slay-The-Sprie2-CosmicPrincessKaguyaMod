using BaseLib.Abstracts;
using BaseLib.Extensions;
using Kaguya.Monsters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using System.Collections.Generic;

namespace Kaguya.Encounters
{
    public sealed class IroPEncounter : CustomEncounterModel
    {
        public override IEnumerable<MonsterModel> AllPossibleMonsters => new[] { ModelDb.Monster<IroP>() };

        // 返回 false 确保不在任何章节的遭遇池中随机出现
        public override bool IsValidForAct(ActModel act) => false;

        public override bool IsWeak => false; // 无关紧要，但必须实现

        public IroPEncounter() : base(RoomType.Monster) { }

        protected override IReadOnlyList<(MonsterModel, string)> GenerateMonsters()
        {
            return new[] { (ModelDb.Monster<IroP>().ToMutable(), (string)null) };
        }
    }
}