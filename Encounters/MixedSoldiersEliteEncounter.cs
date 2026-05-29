using BaseLib.Abstracts;
using BaseLib.Extensions;
using Kaguya.Monsters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using System.Collections.Generic;

namespace Kaguya.Encounters
{
    public class MixedSoldiersEliteEncounter : CustomEncounterModel
    {
        public override IEnumerable<MonsterModel> AllPossibleMonsters => new MonsterModel[]
        {
            ModelDb.Monster<BattleSoldierA>(),
            ModelDb.Monster<BattleSoldierB>()
        };

        public override bool IsValidForAct(ActModel act) => act.ActNumber() == 2; // 第二层

        public override bool IsWeak => false; // 非弱怪，即为强怪/精英

        public MixedSoldiersEliteEncounter() : base(RoomType.Monster) { }

        protected override IReadOnlyList<(MonsterModel, string)> GenerateMonsters()
        {
            // 顺序：士兵A，士兵B，士兵A
            return new[]
            {
                (ModelDb.Monster<BattleSoldierA>().ToMutable(), (string)null),
                (ModelDb.Monster<BattleSoldierB>().ToMutable(), (string)null),
                (ModelDb.Monster<BattleSoldierA>().ToMutable(), (string)null)
            };
        }
    }
}