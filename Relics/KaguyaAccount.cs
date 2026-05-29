using Godot;
using Kaguya.Cards;          // 假设 ILoveMyself 卡牌位于此命名空间
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya
{
    /// <summary>
    /// 遗物“辉夜的账号”：拾起时，将一张 ILoveMyself 加入牌组。
    /// </summary>
    public sealed class KaguyaAccount : RelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;   // 可根据需要调整

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
            HoverTipFactory.FromCardWithCardHoverTips<ILoveMyself>();

        public override bool HasUponPickupEffect => true;

        public override async Task AfterObtained()
        {
            var card = Owner.RunState.CreateCard<ILoveMyself>(Owner);
            if (card != null)
            {
                CardCmd.PreviewCardPileAdd(
                    await CardPileCmd.Add(card, PileType.Deck),
                    2f
                );
            }
        }
    }
}