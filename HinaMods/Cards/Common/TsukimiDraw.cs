using Kaguya.HinaMods.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Kaguya.HinaMods.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.HinaMods.Cards;

/// <summary>
/// 月见抽牌
/// 2费 能力牌 | 自身目标
/// 获得1层月见抽牌。
/// 升级：费用变为1。
/// </summary>
public sealed class TsukimiDraw : HinaModsCard
{
    

    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new PowerVar<TsukimiDrawBuff>(1m);
        }
    }

    public TsukimiDraw()
        : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }
    protected override IEnumerable<IHoverTip> GetCustomHoverTips()
    {
        return new IHoverTip[]
        {
            HoverTipFactory.FromPower<FortunePower>(),
            HoverTipFactory.FromPower<TsukimiDrawBuff>()
        };
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 修复：添加 choiceContext 参数
        await PowerCmd.Apply<TsukimiDrawBuff>(
            choiceContext,
            Owner.Creature,
            DynamicVars["TsukimiDrawBuff"].IntValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}