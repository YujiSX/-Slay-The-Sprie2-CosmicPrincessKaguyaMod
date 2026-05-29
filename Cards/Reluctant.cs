using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class Reluctant : CustomCardModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    private const int baseDebuff = 1;      // 基础易伤/虚弱层数
    private const int overworkAmount = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<VulnerablePower>(baseDebuff),
        new PowerVar<WeakPower>(baseDebuff),
        new PowerVar<Overwork>(overworkAmount)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<Overwork>()
    };

    public Reluctant() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    // 高亮：当目标敌人处于攻击意图时发光（参考眼部攻击）
    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            if (!base.ShouldGlowGoldInternal) return false;
            // 简化高亮：如果有任何敌人攻击意图，不一定针对当前目标，但通常眼部攻击也是只要有攻击意图就高亮，这里精确到当前选中目标更好，但实现复杂，可选
            // 为了简单，返回 true 总是高亮？或者判断当前选中的目标？由于卡牌目标为任意敌人，且高亮在选目标前，只能判断是否存在攻击意图的敌人
            if (CombatState == null) return false;
            return CombatState.HittableEnemies.Any(e => e.Monster?.IntendsToAttack == true);
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        // 如果目标处于攻击意图
        if (target.Monster?.IntendsToAttack == true)
        {
            // 施加易伤
            await PowerCmd.Apply<VulnerablePower>(choiceContext, target, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
            // 施加虚弱
            await PowerCmd.Apply<WeakPower>(choiceContext, target, DynamicVars.Weak.BaseValue, Owner.Creature, this);
        }
        // 自身获得过劳
        await PowerCmd.Apply<Overwork>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级：易伤和虚弱层数各 +1
        DynamicVars.Vulnerable.UpgradeValueBy(1);
        DynamicVars.Weak.UpgradeValueBy(1);
        // 过劳层数不变
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(Reluctant)}.png";
}