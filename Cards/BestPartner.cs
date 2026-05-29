using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.Powers;
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
public class BestPartner : CustomCardModel
{
    private const int energyCost = 2;  // 基础费用2
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseDraw = 1;  // 抽牌数固定为1

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<BestPartnerPower>(baseDraw)
    };

    public BestPartner() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 修正：补充 choiceContext 参数
        await PowerCmd.Apply<BestPartnerPower>(choiceContext, Owner.Creature, DynamicVars["BestPartnerPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级：费用减少1（2 → 1）
        EnergyCost.UpgradeBy(-1);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(BestPartner)}.png";
}
