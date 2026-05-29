using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.Cards; // 引入其他卡牌类型
using Kaguya.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class NeverAccept : CustomCardModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int drawAmount = 5;

    // 未升级时消耗
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public NeverAccept() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(drawAmount)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<Overwork>()
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 移除所有过劳
        var overwork = Owner.Creature.GetPower<Overwork>();
        if (overwork != null)
        {
            await PowerCmd.Remove(overwork);
        }

        // 2. 收集所有需要洗回抽牌堆的卡牌（排除特定卡牌及当前卡自身）
        var allCards = new List<CardModel>();

        // 消耗牌堆（排除特定类型）
        var exhaustPile = PileType.Exhaust.GetPile(Owner);
        allCards.AddRange(exhaustPile.Cards
            .Where(c => !(c is Petition || c is NeverAccept || c is SearchTheWorld))
            .ToList());

        // 弃牌堆（排除特定类型）
        var discardPile = PileType.Discard.GetPile(Owner);
        allCards.AddRange(discardPile.Cards
            .Where(c => !(c is Petition || c is NeverAccept || c is SearchTheWorld))
            .ToList());

        // 手牌（排除当前卡自身及特定类型）
        var handPile = PileType.Hand.GetPile(Owner);
        var handCards = handPile.Cards
            .Where(c => c != this && !(c is Petition || c is NeverAccept || c is SearchTheWorld))
            .ToList();
        allCards.AddRange(handCards);

        // 3. 将所有卡牌移动到抽牌堆
        foreach (var card in allCards)
        {
            await CardPileCmd.Add(card, PileType.Draw);
        }

        // 4. 洗牌
        await CardPileCmd.Shuffle(choiceContext, Owner);

        // 5. 抽5张牌
        await CardPileCmd.Draw(choiceContext, DynamicVars["Cards"].BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(NeverAccept)}.png";
}
