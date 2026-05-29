using BaseLib.Abstracts;
using BaseLib.Extensions;
using Kaguya.Monsters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using System.Collections.Generic;

namespace Kaguya.Encounters
{
    public sealed class TeimeiEncounter : CustomEncounterModel
    {
        public override IEnumerable<MonsterModel> AllPossibleMonsters => new[] { ModelDb.Monster<Teimei>() };
        public override bool IsValidForAct(ActModel act) => false; // 不会随机出现
        public override bool IsWeak => false;

        public TeimeiEncounter() : base(RoomType.Monster) { }

        protected override IReadOnlyList<(MonsterModel, string)> GenerateMonsters()
        {
            return new[] { (ModelDb.Monster<Teimei>().ToMutable(), (string)null) };
        }
    }
}