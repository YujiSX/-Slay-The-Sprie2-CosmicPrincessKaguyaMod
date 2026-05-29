using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class TsukuyomiNightPlus : CustomCardModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Ancient;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true; // 先古卡牌通常在图鉴中不显示，但设为true也无妨

    private const int baseBlock = 10;

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(baseBlock, ValueProp.Move)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromKeyword(TsukuyomiKeyword.Tsukuyomi),
        HoverTipFactory.FromKeyword(RealityKeyword.Reality)
    };

    public TsukuyomiNightPlus() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得格挡
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        // 退出当前姿态（移除现实和月读）
        var reality = Owner.Creature.GetPower<Reality>();
        if (reality != null)
        {
            await PowerCmd.Remove(reality);
        }

        var tsukuyomi = Owner.Creature.GetPower<Tsukuyomi>();
        if (tsukuyomi != null)
        {
            await PowerCmd.Remove(tsukuyomi);
        }

        // 进入月读（补充 choiceContext）
        await PowerCmd.Apply<Tsukuyomi>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级：格挡+4 (10→14)
        DynamicVars.Block.UpgradeValueBy(4);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(TsukuyomiNightPlus)}.png";
}
