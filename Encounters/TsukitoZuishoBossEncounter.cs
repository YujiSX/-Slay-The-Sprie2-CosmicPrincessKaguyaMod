using BaseLib.Abstracts;
using BaseLib.Extensions;
using Kaguya.Monsters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using System.Collections.Generic;

namespace Kaguya.Encounters
{
    public sealed class TsukitoZuishoEliteEncounter : CustomEncounterModel
    {
        public override IEnumerable<MonsterModel> AllPossibleMonsters => new[] { ModelDb.Monster<TsukitoZuisho>() };

        // 在第三层作为精英出现（可根据需要修改）
        public override bool IsValidForAct(ActModel act) => act.ActNumber() == 4;

        // 表示这是一个精英战斗
        public override bool IsWeak => false;

        public TsukitoZuishoEliteEncounter() : base(RoomType.Elite) { }

        protected override IReadOnlyList<(MonsterModel, string)> GenerateMonsters()
        {
            return new[] { (ModelDb.Monster<TsukitoZuisho>().ToMutable(), (string)null) };
        }
    }
}