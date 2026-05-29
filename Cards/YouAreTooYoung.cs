using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
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
public class YouAreTooYoung : CustomCardModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    // 未升级时具有消耗关键词
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => System.Array.Empty<DynamicVar>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromKeyword(MemoryKeyword.Memory)
    };

    public YouAreTooYoung() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 从手牌中选择一张牌
        var handCards = PileType.Hand.GetPile(Owner).Cards.ToList();
        if (handCards.Count == 0) return;

        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this);
        var card = selected.FirstOrDefault();
        if (card == null) return;

        await ProcessRecall(card, choiceContext);
    }

    private async Task ProcessRecall(CardModel card, PlayerChoiceContext choiceContext)
    {
        // 升级卡牌
        CardCmd.Upgrade(card);
        // 消耗卡牌
        await CardCmd.Exhaust(choiceContext, card);

        // 只有成功升级的卡牌才加入召回列表
        if (card.IsUpgraded)
        {
            var recallPower = Owner.Creature.GetPower<RecallPower>();
            if (recallPower == null)
            {
                await PowerCmd.Apply<RecallPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
                recallPower = Owner.Creature.GetPower<RecallPower>();
            }
            recallPower?.AddCard(card);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后移除消耗关键词
        RemoveKeyword(CardKeyword.Exhaust);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(YouAreTooYoung)}.png";
}
