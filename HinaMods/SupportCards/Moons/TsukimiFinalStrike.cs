//using BaseLib.Utils;
//using Godot;
//using Kaguya.HinaMods.Powers;
//using MegaCrit.Sts2.Core.Commands;
//using MegaCrit.Sts2.Core.Entities.Cards;
//using MegaCrit.Sts2.Core.Entities.Creatures;
//using MegaCrit.Sts2.Core.GameActions.Multiplayer;
//using MegaCrit.Sts2.Core.HoverTips;
//using MegaCrit.Sts2.Core.Localization.DynamicVars;
//using MegaCrit.Sts2.Core.Models;
//using MegaCrit.Sts2.Core.Models.Powers;
//using MegaCrit.Sts2.Core.ValueProps;
//using Kaguya.HinaMods;
//using Kaguya.HinaMods.Cards;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Kaguya.HinaMods.Cards.Rare;

//public sealed class HinaModsTsukimiFinalStrike : HinaModsCard
//{
//    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
//    {
//        new DamageVar(50m, ValueProp.Move),
//    };
//    public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

//    // ====================== 核心修复：对齐DailySong悬浮写法 ======================
//    // 保留基类悬浮（MOONVIEW标签提示）+ 追加月见BUFF悬浮提示
//    protected override IEnumerable<IHoverTip> ExtraHoverTips
//    {
//        get
//        {
//            // 1. 先继承基类的所有悬浮提示（自定义标签、基础提示）
//            foreach (var tip in base.ExtraHoverTips)
//                yield return tip;

//            // 2. 追加月见之力的悬浮提示
//            yield return HoverTipFactory.FromPower<TsukimiTimePower>();
//        }
//    }

//    public override HashSet<string> CustomTags => new HashSet<string>() { CustomCardTags.MOONVIEW };

//    public HinaModsTsukimiFinalStrike()
//        : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy) { }

//    // 必须拥有 ≥1000 层月见时间 才可打出
//    protected override bool IsPlayable
//    {
//        get
//        {
//            TsukimiTimePower power = base.Owner.Creature.GetPower<TsukimiTimePower>();
//            return power != null && power.Amount >= 1000;
//        }
//    }

//    // 满足条件发光
//    protected override bool ShouldGlowGoldInternal => IsPlayable;

//    // =========================================================================
//    // 打出逻辑：消耗1000层月见时间 → 造成伤害
//    // =========================================================================
//    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
//    {
//        // 获取月见时间BUFF
//        TsukimiTimePower tsukimiPower = base.Owner.Creature.GetPower<TsukimiTimePower>();

//        // 消耗 1000 层月见时间
//        if (tsukimiPower != null)
//        {
//            await PowerCmd.ModifyAmount(
//                tsukimiPower,
//                -1000,
//                base.Owner.Creature,
//                this
//            );
//        }

//        // 造成伤害
//        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
//    }

//    protected override void OnUpgrade()
//    {
//        base.DynamicVars.Damage.UpgradeValueBy(20m);
//    }
//}