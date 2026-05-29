using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.Enchantments;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class ThoughtStrand : CustomRelicModel
    {
        private bool _isUsed;

        public override bool IsUsedUp => IsUsed;

        public override RelicRarity Rarity => RelicRarity.Ancient;

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
            HoverTipFactory.FromEnchantment<ThoughtEnchantment>();

        [SavedProperty]
        private bool IsUsed
        {
            get => _isUsed;
            set
            {
                AssertMutable();
                _isUsed = value;
                if (IsUsedUp)
                {
                    Status = RelicStatus.Disabled;
                }
            }
        }

        public override string PackedIconPath => "res://images/relics/thought_strand.png";
        protected override string PackedIconOutlinePath => "res://images/relics/thought_strand.png";
        protected override string BigIconPath => "res://images/relics/thought_strand.png";

        public override bool TryModifyCardRewardOptionsLate(
            Player player, List<CardCreationResult> cardRewards, CardCreationOptions options)
        {
            if (player != Owner) return false;
            if (!options.Flags.HasFlag(CardCreationFlags.IsCardReward)) return false;
            if (IsUsed) return false;

            var thought = ModelDb.Enchantment<ThoughtEnchantment>();
            foreach (var cardReward in cardRewards)
            {
                var card = cardReward.Card;
                if (thought.CanEnchant(card))
                {
                    var clonedCard = Owner.RunState.CloneCard(card);
                    CardCmd.Enchant<ThoughtEnchantment>(clonedCard, 1m);
                    cardReward.ModifyCard(clonedCard, this);
                }
            }
            return true;
        }

        public override Task AfterModifyingCardRewardOptions()
        {
            if (IsUsed) return Task.CompletedTask;
            IsUsed = true;
            return Task.CompletedTask;
        }
    }
}