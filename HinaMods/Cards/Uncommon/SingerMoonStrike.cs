using Kaguya.HinaMods.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Kaguya.HinaMods;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.HinaMods.Cards;

// 严格对标官方 BlackHole 卡牌实现
public sealed class SingerMoonStrike : HinaModsCard
{
    // 官方标准：纯净动态变量定义
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new PowerVar<SingerMoonAoEPower>(3m);
        }
    }
    protected override IEnumerable<IHoverTip> GetCustomHoverTips()
    {
        return new IHoverTip[]
        {
            HoverTipFactory.FromPower<SingerMoonAoEPower>(),
            HoverTipFactory.FromPower<FortunePower>()
        };
    }

    // 2费 | 能力牌 | 指向自身
    public SingerMoonStrike()
        : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    // 官方原版打出逻辑：动画 + 施加力量
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // 🔥 唯一修复：补全官方强制参数 choiceContext
        await PowerCmd.Apply<SingerMoonAoEPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(SingerMoonAoEPower)].BaseValue,
            Owner.Creature,
            this);
    }

    // 官方原版升级：费用-1
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}