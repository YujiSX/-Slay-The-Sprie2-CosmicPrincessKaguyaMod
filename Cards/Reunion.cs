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

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class Reunion : CustomCardModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseScry = 3;      // 未升级时伙伴预见3，升级后5
    private const int baseEnergy = 1;    // 获得1能量
    private const int baseDraw = 2;      // 抽2张牌

    // 未升级时具有消耗关键词
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new EnergyVar(baseEnergy),
        new CardsVar(baseDraw),
        new DynamicVar("ScryAmount", baseScry)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromKeyword(PartnerKeyword.Partner),
        HoverTipFactory.FromKeyword(Yujian.yujian),
        base.EnergyHoverTip
    };

    public override IEnumerable<CardTag> Tags => new[] { (CardTag)1004 }; // 伙伴标签

    public Reunion() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

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
        // 先检查伙伴条件并执行预见（在获得能量和抽牌之前）
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

        // 基础效果：获得能量、抽牌
        await PlayerCmd.GainEnergy(DynamicVars["Energy"].BaseValue, Owner);
        await CardPileCmd.Draw(choiceContext, DynamicVars["Cards"].BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        // 升级：伙伴预见数值 +2 (3 → 5)
        DynamicVars["ScryAmount"].UpgradeValueBy(2);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(Reunion)}.png";
}