using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Events;
using System.Collections.Generic;

namespace Kaguya.Cards
{
    [Pool(typeof(EventCardPool))]
    public class YachiyoCup : CustomCardModel
    {
        public override int MaxUpgradeLevel => 0;
        public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Unplayable };

        public YachiyoCup() : base(-1, CardType.Quest, CardRarity.Quest, TargetType.Self) { }

        // 在第二层（Hive）进入下一个事件时，强制替换为八千代杯事件
        public override EventModel ModifyNextEvent(EventModel currentEvent)
        {
            // Hive 章节的索引通常为 2（Overgrowth=0, Underdocks=1, Hive=2, Glory=3）
            if (Owner.RunState.CurrentActIndex == 1)
                return ModelDb.Event<YachiyoCupEvent>();
            return currentEvent;
        }

        public override string PortraitPath => "res://images/packed/card_portraits/kaguya/yachiyo_cup.png";
    }
}