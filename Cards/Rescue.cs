using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using MegaCrit.Sts2.Core.Combat;
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
public class Rescue : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseBlock = 4;  

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(baseBlock, ValueProp.Move)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromKeyword(PartnerKeyword.Partner)
    };

    public override IEnumerable<CardTag> Tags => new[] { (CardTag)1004 };

    public Rescue() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    // 高亮：满足伙伴条件时发光（独立于基类）
    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            var lastCard = GetPreviousPlayedCard();
            if (lastCard == null) return false;
            bool isPartner = lastCard.Tags.Contains((CardTag)1004);
            return !isPartner && lastCard.Type == this.Type;
        }
    }

    private CardModel GetPreviousPlayedCard()
    {
        if (CombatState == null) return null;
        var entry = CombatManager.Instance.History.CardPlaysStarted
            .LastOrDefault(e => e.CardPlay.Card.Owner == this.Owner && e.CardPlay.Card != this);
        return entry?.CardPlay.Card;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 基础格挡
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        // 伙伴效果：再获得一次格挡
        var lastCard = GetPreviousPlayedCard();
        bool isPartnerActive = false;
        if (lastCard != null)
        {
            bool isPartner = lastCard.Tags.Contains((CardTag)1004);
            if (!isPartner && lastCard.Type == this.Type)
                isPartnerActive = true;
        }

        if (isPartnerActive)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(Rescue)}.png";
}
