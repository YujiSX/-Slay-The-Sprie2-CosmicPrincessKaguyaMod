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
    public sealed class YachiyoShrine : RelicModel
    {
        // 稀有度：先古（通常通过事件获取）
        public override RelicRarity Rarity => RelicRarity.Ancient;

        // 悬浮提示：显示会添加的卡牌（Remember）
        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
            HoverTipFactory.FromCardWithCardHoverTips<Remember>();

        // 声明拾起时有效果
        public override bool HasUponPickupEffect => true;

        // 拾起时触发：将一张“Remember”加入牌组
        public override async Task AfterObtained()
        {
            // 创建 Remember 卡牌实例（与当前战斗无关，加入牌组）
            var card = Owner.RunState.CreateCard<Remember>(Owner);
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