using BaseLib.Abstracts;
using BaseLib.Extensions;
using Kaguya.Monsters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using System.Collections.Generic;

namespace Kaguya.Encounters
{
    public class OxDemonArthropodEncounter : CustomEncounterModel
    {
        public override IEnumerable<MonsterModel> AllPossibleMonsters => new[] { ModelDb.Monster<OxDemonArthropod>() };

        public override bool IsValidForAct(ActModel act) => act.ActNumber() == 2;

        public override bool IsWeak => false;

        public OxDemonArthropodEncounter() : base(RoomType.Elite) { }

        protected override IReadOnlyList<(MonsterModel, string)> GenerateMonsters()
        {
            return new[] { (ModelDb.Monster<OxDemonArthropod>().ToMutable(), (string)null) };
        }
    }
}