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
public class OnlyEachOther : CustomCardModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseScry = 3;

    // 未升级时具有消耗关键词
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("ScryAmount", baseScry)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromKeyword(TsukuyomiKeyword.Tsukuyomi),
        HoverTipFactory.FromKeyword(RealityKeyword.Reality),
        HoverTipFactory.FromKeyword(PartnerKeyword.Partner),
        HoverTipFactory.FromKeyword(Yujian.yujian)
    };

    public override IEnumerable<CardTag> Tags => new[] { (CardTag)1004 }; // 伙伴标签

    public OnlyEachOther() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

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
        // 检查是否处于月读状态
        bool hasTsukuyomi = Owner.Creature.Powers.Any(p => p is Tsukuyomi);

        if (hasTsukuyomi)
        {
            // 进入现实
            await PowerCmd.Apply<Reality>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        }
        else
        {
            // 进入月读
            await PowerCmd.Apply<Tsukuyomi>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        }

        // 伙伴效果：预见
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
            int scryAmount = (int)DynamicVars["ScryAmount"].BaseValue;
            await ScryHelper.Scry(Owner, scryAmount, choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：预见数增加2（3 → 5）
        DynamicVars["ScryAmount"].UpgradeValueBy(2);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(OnlyEachOther)}.png";
}
