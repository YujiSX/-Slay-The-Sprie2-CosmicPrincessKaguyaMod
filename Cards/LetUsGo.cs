using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.Powers;
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
public class LetUsGo : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseDraw = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(baseDraw)
    };

    // 伙伴关键词
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromKeyword(PartnerKeyword.Partner),
        HoverTipFactory.FromKeyword(TsukuyomiKeyword.Tsukuyomi)
    };

    // 伙伴标识 tag
    public override IEnumerable<CardTag> Tags => new[] { (CardTag)1004 };

    public LetUsGo() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

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

    // 获取上一张打出的牌（排除当前卡）
    private CardModel GetPreviousPlayedCard()
    {
        if (CombatState == null) return null;
        var entry = CombatManager.Instance.History.CardPlaysStarted
            .LastOrDefault(e => e.CardPlay.Card.Owner == this.Owner && e.CardPlay.Card != this);
        return entry?.CardPlay.Card;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 进入月读
        await PowerCmd.Apply<Tsukuyomi>(choiceContext, Owner.Creature, 1, Owner.Creature, this);

        // 检查伙伴条件
        var lastCard = GetPreviousPlayedCard();
        bool isPartnerActive = false;
        if (lastCard != null)
        {
            bool isPartner = lastCard.Tags.Contains((CardTag)1004);
            if (!isPartner && lastCard.Type == this.Type)
                isPartnerActive = true;
        }

        // 伙伴效果：抽牌
        if (isPartnerActive)
        {
            int drawAmount = (int)DynamicVars["Cards"].BaseValue;
            await CardPileCmd.Draw(choiceContext, drawAmount, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：抽牌数 +1
        DynamicVars["Cards"].UpgradeValueBy(1);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(LetUsGo)}.png";
}
