using BaseLib.Abstracts;
using BaseLib.Extensions;
using Kaguya.Monsters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using System.Collections.Generic;

namespace Kaguya.Encounters
{
    public class OxDemonEncounter : CustomEncounterModel
    {
        public override IEnumerable<MonsterModel> AllPossibleMonsters => new[] { ModelDb.Monster<OxDemon>() };

        public override bool IsValidForAct(ActModel act) => act.ActNumber() == 1; // 第一幕

        public override bool IsWeak => false; // 精英怪

        public OxDemonEncounter() : base(RoomType.Elite) { }

        protected override IReadOnlyList<(MonsterModel, string)> GenerateMonsters()
        {
            return new[] { (ModelDb.Monster<OxDemon>().ToMutable(), (string)null) };
        }
    }
}