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
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class Entry : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseVigor = 6;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<VigorPower>(baseVigor)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromKeyword(TsukuyomiKeyword.Tsukuyomi),
        HoverTipFactory.FromPower<VigorPower>()
    };

    public Entry() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 检查是否处于月读状态
        bool hasTsukuyomi = Owner.Creature.Powers.Any(p => p is Tsukuyomi);

        if (hasTsukuyomi)
        {
            // 获得活力
            int vigorAmount = (int)DynamicVars["VigorPower"].BaseValue;
            await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, vigorAmount, Owner.Creature, this);
        }
        else
        {
            // 进入月读
            await PowerCmd.Apply<Tsukuyomi>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：活力增加2（6 → 8）
        DynamicVars["VigorPower"].UpgradeValueBy(2);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(Entry)}.png";
}
