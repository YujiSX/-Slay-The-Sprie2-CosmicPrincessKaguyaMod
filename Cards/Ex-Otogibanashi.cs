using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.GameInfo.Objects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.Cards;

[Pool(typeof(EventCardPool))]
public sealed class ExOtogibanashi : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Ancient;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public override IEnumerable<CardTag> Tags => new[] { (CardTag)1001 };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("ScryAmount", 8),
        new DynamicVar("DrawAmount", 3)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromCard<YachiyoOtogibanashi>(),
        HoverTipFactory.FromKeyword(Yujian.yujian)
    };

    public ExOtogibanashi() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int scryAmount = (int)DynamicVars["ScryAmount"].BaseValue;
        await ScryHelper.Scry(Owner, scryAmount, choiceContext);

        int drawAmount = (int)DynamicVars["DrawAmount"].BaseValue;
        await CardPileCmd.Draw(choiceContext, drawAmount, Owner);

        // 使用 CombatState.CreateCard 创建与战斗关联的 Remember 实例
        var YachiyoExCard = base.CombatState.CreateCard<YachiyoOtogibanashi>(base.Owner);
        if (YachiyoExCard != null)
        {
            if (IsUpgraded)
            {
                CardCmd.Upgrade(YachiyoExCard);
            }
            // 修正：移除 addedByPlayer 参数，改为传递 Owner 作为 creator
            await CardPileCmd.AddGeneratedCardToCombat(YachiyoExCard, PileType.Draw, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        // 可根据需要增加升级效果
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/event/{nameof(ExOtogibanashi)}.png";
}