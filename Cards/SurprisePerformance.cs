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
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class SurprisePerformance : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseCreation = 3;      // 未升级下回合获得3层
    private const int partnerCreation = 2;    // 伙伴立即获得1层

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<CreationPower>("CreationPower", baseCreation)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromKeyword(PartnerKeyword.Partner),
        HoverTipFactory.FromPower<CreationPower>()
    };

    public override IEnumerable<CardTag> Tags => new[] { (CardTag)1004 };

    public SurprisePerformance() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    // 高亮：满足伙伴条件时发光（独立于基类）
    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            var history = CombatManager.Instance.History.CardPlaysStarted;
            var lastEntry = history.LastOrDefault(e => e.CardPlay.Card.Owner == this.Owner && e.CardPlay.Card != this);
            if (lastEntry == null) return false;
            var lastCard = lastEntry.CardPlay.Card;
            bool isPartner = lastCard.Tags.Contains((CardTag)1004);
            return !isPartner && lastCard.Type == this.Type;
        }
    }

    // 伙伴条件检测（供 OnPlay 使用）
    private bool IsPartnerActive()
    {
        var history = CombatManager.Instance.History.CardPlaysStarted;
        var lastEntry = history.LastOrDefault(e => e.CardPlay.Card.Owner == this.Owner && e.CardPlay.Card != this);
        if (lastEntry == null) return false;
        var lastCard = lastEntry.CardPlay.Card;
        bool isPartner = lastCard.Tags.Contains((CardTag)1004);
        return !isPartner && lastCard.Type == this.Type;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool partnerActive = IsPartnerActive();

        // 伙伴效果：立即获得1层创作
        if (partnerActive)
        {
            await PowerCmd.Apply<CreationPower>(choiceContext, Owner.Creature, partnerCreation, Owner.Creature, this);
        }

        // 下回合开始时获得创作（施加一个延迟能力）
        int delayedAmount = (int)DynamicVars["CreationPower"].BaseValue;
        await PowerCmd.Apply<SurprisePerformancePower>(choiceContext, Owner.Creature, delayedAmount, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级：延迟获得的创作层数 +1 (3→4)
        DynamicVars["CreationPower"].UpgradeValueBy(1);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(SurprisePerformance)}.png";
}
