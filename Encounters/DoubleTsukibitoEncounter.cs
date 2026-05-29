using BaseLib.Abstracts;
using BaseLib.Extensions;
using Kaguya.Monsters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using System.Collections.Generic;

namespace Kaguya.Encounters
{
    public class DoubleTsukibitoEncounter : CustomEncounterModel
    {
        public override IEnumerable<MonsterModel> AllPossibleMonsters => new[] { ModelDb.Monster<Tsukibito>() };

        public override bool IsValidForAct(ActModel act) => act.ActNumber() == 3; // 第三幕

        public override bool IsWeak => true; // 弱怪池

        public DoubleTsukibitoEncounter() : base(RoomType.Monster) { }

        protected override IReadOnlyList<(MonsterModel, string)> GenerateMonsters()
        {
            // 生成两只月人，不指定槽位，游戏自动分配位置
            return new[]
            {
                (ModelDb.Monster<Tsukibito>().ToMutable(), (string)null),
                (ModelDb.Monster<Tsukibito2>().ToMutable(), (string)null)
            };
        }
    }
}