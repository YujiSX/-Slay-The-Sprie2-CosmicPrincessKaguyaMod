using Godot;
using Kaguya.Cards;
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
    public sealed class OrdinaryHappiness : RelicModel
    {
        // 稀有度：先古（通常通过事件获取）
        public override RelicRarity Rarity => RelicRarity.Ancient;

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
            HoverTipFactory.FromCardWithCardHoverTips<Melt>();

        // 声明拾起时有效果
        public override bool HasUponPickupEffect => true;

        public override async Task AfterObtained()
        {
            var card = Owner.RunState.CreateCard<Melt>(Owner);
            if (card != null)
            {
                // 将卡牌加入牌组（Deck），并播放预览动画（时长2秒）
                CardCmd.PreviewCardPileAdd(
                    await CardPileCmd.Add(card, PileType.Deck),
                    2f
                );
            }
        }
    }
}