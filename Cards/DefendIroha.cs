using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class DefendIroha : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Basic;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseBlock = 5;

    public override bool GainsBlock => true;  // 用于触发某些与格挡相关的特效

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(baseBlock, ValueProp.Move)
    };

    // 防御标签（用于防御木偶等效果）
    public override IEnumerable<CardTag> Tags => new[] { CardTag.Defend };

    public DefendIroha() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(DefendIroha)}.png";
}
