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
public class ToughItOut : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseBlock = 15;
    private const int overworkAmount = 2;

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(baseBlock, ValueProp.Move),
        new PowerVar<Overwork>(overworkAmount)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<Overwork>(),
        HoverTipFactory.FromCard<Boring>()
    };

    public ToughItOut() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得格挡
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        // 获得过劳（补充 choiceContext）
        await PowerCmd.Apply<Overwork>(choiceContext, Owner.Creature, 2, Owner.Creature, this);

        // 添加一张枯燥卡牌（修正签名，移除 addedByPlayer 参数）
        var boringCard = base.CombatState.CreateCard<Boring>(Owner);
        if (boringCard != null)
        {
            await CardPileCmd.AddGeneratedCardToCombat(boringCard, PileType.Hand, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(ToughItOut)}.png";
}
