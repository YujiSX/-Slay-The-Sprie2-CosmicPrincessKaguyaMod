using Godot;
using Kaguya.Cards;          // 假设“星降之海”卡牌类位于此命名空间下
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
    /// 遗物“月夜见”：拾起时，将一张“星降之海”加入牌组。
    /// </summary>
    public sealed class TsukuyomiShrine : RelicModel
    {
        // 稀有度（可根据需要调整，如 RelicRarity.Ancient）
        public override RelicRarity Rarity => RelicRarity.Ancient;

        // 悬浮提示：显示会添加的卡牌（星降之海）
        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
            HoverTipFactory.FromCardWithCardHoverTips<StarlitSea>();  // 假设卡牌类名为 StarlightSea

        // 声明拾起时有效果
        public override bool HasUponPickupEffect => true;

        // 拾起时触发：将一张“星降之海”加入牌组
        public override async Task AfterObtained()
        {
            // 创建“星降之海”卡牌实例（与当前战斗无关，加入牌组）
            var card = Owner.RunState.CreateCard<StarlitSea>(Owner);
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