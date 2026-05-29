using BaseLib.Utils;
using Godot;
using Kaguya.HinaMods.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
/// 松饼
/// 1费 技能牌 | 自身目标
/// 获得3层月夜，治疗3点生命。消耗。
/// 升级：月夜变为4层，治疗变为4点。
/// </summary>
public sealed class HinaModsMuffin : HinaModsCard
{
    public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<FortunePower>(3m),
        new HealVar(3m)
    };

    protected override IEnumerable<IHoverTip> GetCustomHoverTips()
    {
        return new IHoverTip[]
        {
           HoverTipFactory.FromPower<FortunePower>(),
        };
    }

    public HinaModsMuffin()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 修复：添加 choiceContext 参数
        await PowerCmd.Apply<FortunePower>(
            choiceContext,
            base.Owner.Creature,
            base.DynamicVars["FortunePower"].BaseValue,
            base.Owner.Creature,
            this
        );

        await CreatureCmd.Heal(base.Owner.Creature, base.DynamicVars.Heal.IntValue);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["FortunePower"].UpgradeValueBy(1m);
        base.DynamicVars.Heal.UpgradeValueBy(1m);
    }
}