using BaseLib.Utils;
using Godot;
using Kaguya.HinaMods.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using Kaguya.HinaMods;
using Kaguya.HinaMods.Cards;
using Kaguya.HinaMods.Relics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.HinaMods.Relics;

/// <summary>
/// 月亮头饰
/// 专属遗物
/// 每回合第一次打出卡牌获得月夜时，额外获得2层月夜
/// </summary>
public sealed class MoonHeaddressRelic : HinaRelics
{
    // ====================== 基础配置 ======================
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override bool ShowCounter => false;

    // 图标路径
    public override string PackedIconPath => "res://images/hinamods/relics/moon_headdress_relic.png";
    protected override string PackedIconOutlinePath => "res://images/hinamods/relics/moon_headdress_relic.png";
    protected override string BigIconPath => "res://images/hinamods/relics/moon_headdress_relic.png";

    // 回合触发标记（对标参考代码格式）
    private bool _hasTriggeredThisTurn;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<FortunePower>()
    };

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        // 完全复刻你的参考代码格式
        if (!participants.Contains(Owner.Creature))
            return Task.CompletedTask;

        // 回合开始重置触发标记
        _hasTriggeredThisTurn = false;
        return Task.CompletedTask;
    }

    // ====================== 【参考2】监听力量层数变化 ======================
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature applier, CardModel cardSource)
    {
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);

        // 🔥【修复】必须是【自己身上】的月夜Buff，不是则返回
        if (power.Owner != Owner.Creature)
            return;

        // 2. 必须是月夜(FortunePower)
        if (power is not FortunePower)
            return;

        // 3. 仅【获得层数】时触发（失去层数不触发）
        if (amount <= 0)
            return;

        // 4. 必须是【打出卡牌】获得的月夜（核心判定）
        if (cardSource == null)
            return;

        // 5. 每回合仅触发一次
        if (_hasTriggeredThisTurn)
            return;

        // 标记已触发
        _hasTriggeredThisTurn = true;

        // 额外获得2层月夜（完全照搬你的参考代码）
        await PowerCmd.Apply<FortunePower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            2m,
            Owner.Creature,
            null
        );
        Flash();
    }

    // ====================== 固定空实现 ======================
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.AfterCardPlayed(choiceContext, cardPlay);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        return Task.CompletedTask;
    }
}