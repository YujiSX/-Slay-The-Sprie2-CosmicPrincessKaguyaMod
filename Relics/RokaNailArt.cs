using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.Cards;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class RokaNailArt : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;

        protected override IEnumerable<DynamicVar> CanonicalVars => new[]
        {
            new CardsVar(3)
        };

        protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<NailArt>();

        public override string PackedIconPath => "res://images/relics/roka_nail_art.png";
        protected override string PackedIconOutlinePath => "res://images/relics/roka_nail_art.png";
        protected override string BigIconPath => "res://images/relics/roka_nail_art.png";

        public override async Task AfterObtained()
        {
            // 使用对象初始值设定项，解决 init-only 属性赋值问题
            var prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 0, (int)DynamicVars["Cards"].BaseValue)
            {
                Cancelable = false,
                RequireManualConfirmation = true
            };

            var selectedCards = await CardSelectCmd.FromDeckForTransformation(
                Owner,
                prefs,
                (CardModel original) => new CardTransformation(original, CreateNailArtFromOriginal(original, forPreview: true))
            );

            var transformations = selectedCards
                .Select(original => new CardTransformation(original, CreateNailArtFromOriginal(original, forPreview: false)))
                .ToList();

            await CardCmd.Transform(transformations, Owner.PlayerRng.Transformations);
        }

        private CardModel CreateNailArtFromOriginal(CardModel original, bool forPreview)
        {
            CardModel nailArt = forPreview
                ? ModelDb.Card<NailArt>().ToMutable()
                : Owner.RunState.CreateCard<NailArt>(Owner);

            if (original.IsUpgraded && nailArt.IsUpgradable)
            {
                if (forPreview)
                    nailArt.UpgradeInternal();
                else
                    CardCmd.Upgrade(nailArt);
            }

            if (original.Enchantment != null)
            {
                var enchantment = (EnchantmentModel)original.Enchantment.MutableClone();
                if (enchantment.CanEnchant(nailArt))
                {
                    if (forPreview)
                    {
                        nailArt.EnchantInternal(enchantment, enchantment.Amount);
                        enchantment.ModifyCard();
                    }
                    else
                    {
                        CardCmd.Enchant(enchantment, nailArt, enchantment.Amount);
                    }
                }
            }

            return nailArt;
        }
    }
}