using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.Cards;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class EightThousandYearsLonging : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;


        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
            HoverTipFactory.FromCardWithCardHoverTips<Determination>(upgrade: true);

        public override string PackedIconPath => "res://images/relics/eight_thousand_years_longing.png";
        protected override string PackedIconOutlinePath => "res://images/relics/eight_thousand_years_longing.png";
        protected override string BigIconPath => "res://images/relics/eight_thousand_years_longing.png";

        public override async Task AfterObtained()
        {
            var determination = Owner.RunState.CreateCard<Determination>(Owner);
            if (determination != null)
            {
                if (determination.IsUpgradable)
                    CardCmd.Upgrade(determination);

                var addResult = await CardPileCmd.Add(determination, PileType.Deck);
                CardCmd.PreviewCardPileAdd(new[] { addResult }, 2f);
            }
        }
    }
}
