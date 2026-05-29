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
public class TaketoriMonogatari : CustomCardModel
{
    private const int energyCost = 3;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public TaketoriMonogatari() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromKeyword(PartnerKeyword.Partner),
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => System.Array.Empty<DynamicVar>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 施加能力
        await PowerCmd.Apply<TaketoriMonogatariPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);

        // 获取刚施加的能力实例并设置升级标志
        var power = Owner.Creature.GetPower<TaketoriMonogatariPower>();
        if (power != null)
        {
            power.ShouldUpgradeGeneratedCards = this.IsUpgraded;
        }
    }

    protected override void OnUpgrade()
    {
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(TaketoriMonogatari)}.png";
}
