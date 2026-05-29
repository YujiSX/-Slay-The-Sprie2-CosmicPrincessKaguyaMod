using BaseLib.Abstracts;
using BaseLib.Extensions;
using Kaguya.Monsters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using System.Collections.Generic;

namespace Kaguya.Encounters
{
    public sealed class NayEncounter : CustomEncounterModel
    {
        public override IEnumerable<MonsterModel> AllPossibleMonsters => new[] { ModelDb.Monster<Nay>() };
        public override bool IsValidForAct(ActModel act) => false; // 不会随机出现
        public override bool IsWeak => false;

        public NayEncounter() : base(RoomType.Monster) { }

        protected override IReadOnlyList<(MonsterModel, string)> GenerateMonsters()
        {
            return new[] { (ModelDb.Monster<Nay>().ToMutable(), (string)null) };
        }
    }
}