using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class Guard : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseBuffer = 1;
    private const int overworkAmount = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<BufferPower>(baseBuffer),
        new PowerVar<Overwork>(overworkAmount)
    };

    // 未升级时拥有虚无和消耗，升级后移除虚无（保留消耗）
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Ethereal, CardKeyword.Exhaust };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<BufferPower>(),
        HoverTipFactory.FromPower<Overwork>()
    };

    public Guard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BufferPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        await PowerCmd.Apply<Overwork>(choiceContext, Owner.Creature, 3, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级后移除虚无关键词
        RemoveKeyword(CardKeyword.Ethereal);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(Guard)}.png";
}
