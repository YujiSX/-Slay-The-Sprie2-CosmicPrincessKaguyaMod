using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.Cards; // 确保能访问 Petition, NeverAccept, SearchTheWorld
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class SearchTheWorld : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public SearchTheWorld() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => System.Array.Empty<DynamicVar>();

    private async Task<CardModel> SelectCardFromPile(PileType pileType, PlayerChoiceContext choiceContext)
    {
        var pile = pileType.GetPile(Owner);
        // 过滤掉被排除的卡牌类型
        var cards = pile.Cards
            .Where(c => !(c is Petition || c is NeverAccept || c is SearchTheWorld))
            .ToList();
        if (cards.Count == 0) return null;

        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
        var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, cards, Owner, prefs);
        return selected.FirstOrDefault();
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 从抽牌堆选择一张牌
        var drawCard = await SelectCardFromPile(PileType.Draw, choiceContext);
        if (drawCard != null)
        {
            CardCmd.ApplyKeyword(drawCard, CardKeyword.Ethereal);
            await CardPileCmd.Add(drawCard, PileType.Hand);
        }

        // 从弃牌堆选择一张牌
        var discardCard = await SelectCardFromPile(PileType.Discard, choiceContext);
        if (discardCard != null)
        {
            CardCmd.ApplyKeyword(discardCard, CardKeyword.Ethereal);
            await CardPileCmd.Add(discardCard, PileType.Hand);
        }

        // 从消耗牌堆选择一张牌
        var exhaustCard = await SelectCardFromPile(PileType.Exhaust, choiceContext);
        if (exhaustCard != null)
        {
            CardCmd.ApplyKeyword(exhaustCard, CardKeyword.Ethereal);
            await CardPileCmd.Add(exhaustCard, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(SearchTheWorld)}.png";
}