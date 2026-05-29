using Kaguya.HinaMods.Powers;
using Kaguya.HinaMods.SupportCards.Common;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.HinaMods.Cards;

public sealed class MultiSupportGather : HinaModsCard
{
    // 多人联机专属
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<IHoverTip> GetCustomHoverTips()
    {
        return new IHoverTip[]
        {
        HoverTipFactory.FromCard<SupportHeavyBlock>(),
        HoverTipFactory.FromCard<SupportDraw>(),
        HoverTipFactory.FromCard<SupportStrike>()
        };
    }
    // 2费 技能牌 稀有 一名友方
    public MultiSupportGather()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.AllAllies)
    { }

    // 官方原生遍历逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IEnumerable<Creature> teammates = from c in CombatState.GetTeammatesOf(Owner.Creature)
                                          where c != null && c.IsAlive && c.IsPlayer
                                          select c;

        foreach (Creature playerCreature in teammates)
        {
            // 1. 招架
            CardModel blockCard = CardScope.CreateCard<SupportHeavyBlock>(playerCreature.Player);

            await CardPileCmd.Add(blockCard, PileType.Hand);

            // 2. 疾驰
            CardModel drawCard = CardScope.CreateCard<SupportDraw>(playerCreature.Player);
            await CardPileCmd.Add(drawCard, PileType.Hand);

            // 🔥 新增：支援打击
            CardModel strikeCard = CardScope.CreateCard<SupportStrike>(playerCreature.Player);
            await CardPileCmd.Add(strikeCard, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}