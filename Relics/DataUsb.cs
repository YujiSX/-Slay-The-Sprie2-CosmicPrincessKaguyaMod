using Godot;
using Kaguya.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    public sealed class DataUsb : RelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
            HoverTipFactory.FromCardWithCardHoverTips<BackToWall>();

        public override bool HasUponPickupEffect => true;

        public override async Task AfterObtained()
        {
            var card = Owner.RunState.CreateCard<BackToWall>(Owner);
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