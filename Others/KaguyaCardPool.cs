using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Kaguya.CardPools
{
    public class KaguyaCardPool : CustomCardPoolModel
    {
        // 卡池的ID。必须唯一防撞车。
        public override string Title => "Iroha";

        // 描述中使用的能量图标。大小为24x24。
        public override string TextEnergyIconPath => "res://images/ui/energys/kaguya_energy_icon.png";
        // tooltip和卡牌左上角的能量图标。大小为74x74。
        public override string BigEnergyIconPath => "res://images/ui/energys/kaguya_energy_icon_resized.png";

        // 卡池的主题色。
        public override Color DeckEntryCardColor => new(0.5f, 0.5f, 1f);

        // 如果你使用默认的卡框，可以使用这个颜色来修改卡框的颜色。
        //public override Color ShaderColor => new(0.5f, 0.5f, 1f);

        // 如果你使用自定义卡框图片，重写CustomFrame方法并返回你的卡框图片。
        public override Texture2D CustomFrame(CustomCardModel card)
        {
            return card.Type switch
            {
                CardType.Attack => PreloadManager.Cache.GetAsset<Texture2D>("res://images/ui/card/Attack.png"),
                CardType.Power => PreloadManager.Cache.GetAsset<Texture2D>("res://images/ui/card/Power.png"),
                _ => PreloadManager.Cache.GetAsset<Texture2D>("res://images/ui/card/Skill.png"),
            };
        }

        // 卡池是否是无色。例如事件、状态等卡池就是无色的。
        public override bool IsColorless => false;
        public override string CardFrameMaterialPath => "irohacard";
    }
}