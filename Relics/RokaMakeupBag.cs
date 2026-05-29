using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class RokaMakeupBag : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;

        // BaseLib 要求：提供图标路径（请根据实际资源路径调整）
        public override string PackedIconPath => "res://images/relics/roka_makeup_bag.png";
        protected override string PackedIconOutlinePath => "res://images/relics/roka_makeup_bag_outline.png";
        protected override string BigIconPath => "res://images/relics/roka_makeup_bag_big.png";

        // 动态变量（用于本地化描述）
        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new CardsVar(3)
        };

        // 悬浮提示
        protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
        {
            HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
        };

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner) return;
            if (!CombatManager.Instance.IsInProgress) return;

            Flash();

            var allCards = Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint);
            int count = (int)DynamicVars["Cards"].BaseValue;
            var randomCards = CardFactory.GetDistinctForCombat(Owner, allCards, count, Owner.RunState.Rng.CombatCardGeneration).ToList();

            if (randomCards.Count == 0) return;

            var prefs = new CardSelectorPrefs(L10NLookup("ROKA_MAKEUP_BAG.selectionPrompt"), 1);
            var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, randomCards, Owner, prefs);
            var selectedCard = selected.FirstOrDefault();
            if (selectedCard == null) return;

            CardCmd.ApplyKeyword(selectedCard, CardKeyword.Ethereal);
            await CardPileCmd.AddGeneratedCardToCombat(selectedCard, PileType.Hand, Owner);
        }
    }
}