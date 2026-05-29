using BaseLib.Utils;
using Godot;
using Kaguya.HinaMods.Powers;
using Kaguya.HinaMods.SupportCards.Common;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Kaguya.HinaMods.Cards;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.HinaMods.Cards.Uncommon;

public sealed class HomeRun : HinaModsCard
{
    private const decimal BaseDamage = 5m;
    private const decimal BaseIncreasePerSupport = 2m;

    // 构造函数：1费 攻击 稀有 全体敌人（完全保持不变）
    public HomeRun()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
    }
    protected override IEnumerable<IHoverTip> GetCustomHoverTips()
    {
        return new IHoverTip[]
        {
            HoverTipFactory.FromCard<SupportStrike>()
        };
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(BaseDamage),
        new ExtraDamageVar(0m),
        new IntVar("IncreasePerSupport", BaseIncreasePerSupport),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((CardModel card, Creature _) => 1)
    ];

    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Strike };

    // 🔥 核心修复：完全对标 FullMoonSweepCard 的官方全体攻击写法
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 官方通用攻击方法，自动处理全体/单体目标，无空值报错
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
    }

    // 完全保留你的支援打击增伤逻辑（无改动）
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.AfterCardPlayed(choiceContext, cardPlay);
        if (cardPlay.Card.Owner != Owner || cardPlay.Card is not SupportStrike)
            return;

        decimal add = DynamicVars["IncreasePerSupport"].BaseValue;
        DynamicVars.CalculationBase.BaseValue += add;
    }

    // 完全保留你的升级逻辑（无改动）
    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(3m);
    }
}