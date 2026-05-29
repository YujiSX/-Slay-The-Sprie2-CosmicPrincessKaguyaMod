using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using Kaguya.HinaMods.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Kaguya.HinaMods;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.HinaMods.Cards;

/// <summary>
/// 飞跃
/// 2费 技能牌 | 自身目标
/// 获得10点格挡，获得1层飞跃，结束回合。消耗。
/// 升级：保留。
/// </summary>
public sealed class HinaModsLeap : HinaModsCard
{
    public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(10m, ValueProp.Move)
    };
    protected override IEnumerable<IHoverTip> GetCustomHoverTips()
    {
        return new IHoverTip[]
        {
            HoverTipFactory.FromCard<SupportDexterity>(),
            HoverTipFactory.FromCard<SupportStrength>()
        };
    }

    protected override bool IsPlayable => base.IsPlayable;
    protected override bool ShouldGlowGoldInternal => false;

    public HinaModsLeap() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Player player = Owner;
        if (player == null) return;

        await CreatureCmd.TriggerAnim(player.Creature, "Cast", player.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, cardPlay);

        // 修复：添加 choiceContext 参数
        await PowerCmd.Apply<LeadPower>(
            choiceContext,
            player.Creature,
            1,
            player.Creature,
            this);

        PlayerCmd.EndTurn(player, canBackOut: false);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}