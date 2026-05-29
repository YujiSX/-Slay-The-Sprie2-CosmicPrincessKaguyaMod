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
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class CheerUp : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override bool GainsBlock => true;

    // 动态变量：每层格挡值（基础6，升级后8）
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(6, ValueProp.Move)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<Overwork>()
    };

    public CheerUp() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var overwork = Owner.Creature.GetPower<Overwork>();
        int overworkAmount = overwork != null ? (int)overwork.Amount : 0;

        if (overworkAmount > 0)
        {
            if (overwork != null)
                await PowerCmd.Remove(overwork);

            int blockPerLayer = (int)DynamicVars["Block"].BaseValue;
            int totalBlock = overworkAmount * blockPerLayer;
            var blockVar = new BlockVar(totalBlock, ValueProp.Move);
            await CreatureCmd.GainBlock(Owner.Creature, blockVar, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：每层格挡值增加2（6 → 8）
        DynamicVars["Block"].UpgradeValueBy(2);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(CheerUp)}.png";
}
