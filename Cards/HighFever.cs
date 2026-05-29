using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
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
public class HighFever : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseBlockPerOverwork = 3;  // 未升级时每层3点格挡

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(baseBlockPerOverwork, ValueProp.Move)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<Overwork>()
    };

    public HighFever() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 先获得1层过劳
        await PowerCmd.Apply<Overwork>(choiceContext, Owner.Creature, 1, Owner.Creature, this);

        // 获取当前过劳层数（包含刚获得的这一层）
        int overworkAmount = 0;
        var overwork = Owner.Creature.GetPower<Overwork>();
        if (overwork != null)
        {
            overworkAmount = (int)overwork.Amount;
        }

        if (overworkAmount > 0)
        {
            int blockPerOverwork = (int)DynamicVars["Block"].BaseValue;
            // 多次给予格挡，每次单独调用，确保敏捷等修正正常生效
            for (int i = 0; i < overworkAmount; i++)
            {
                var blockVar = new BlockVar(blockPerOverwork, ValueProp.Move);
                await CreatureCmd.GainBlock(Owner.Creature, blockVar, cardPlay);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：每次格挡值增加2 (3 → 5)
        DynamicVars["Block"].UpgradeValueBy(2);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(HighFever)}.png";
}
