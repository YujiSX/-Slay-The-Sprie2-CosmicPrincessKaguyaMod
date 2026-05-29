using BaseLib.Abstracts;
using BaseLib.Extensions;
using Kaguya.Monsters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using System.Collections.Generic;

namespace Kaguya.Encounters
{
    public sealed class TsukitoBagEncounter : CustomEncounterModel
    {
        public override IEnumerable<MonsterModel> AllPossibleMonsters => new[] { ModelDb.Monster<TsukitoBag>() };

        public override bool IsValidForAct(ActModel act) => act.ActNumber() == 3; // 只在第三层出现

        public override bool IsWeak => false; // 不是弱怪池

        public TsukitoBagEncounter() : base(RoomType.Elite) { } // 精英房间

        protected override IReadOnlyList<(MonsterModel, string)> GenerateMonsters()
        {
            // 单体精英，不指定槽位（null表示自动分配）
            return new[] { (ModelDb.Monster<TsukitoBag>().ToMutable(), (string)null) };
        }
    }
}