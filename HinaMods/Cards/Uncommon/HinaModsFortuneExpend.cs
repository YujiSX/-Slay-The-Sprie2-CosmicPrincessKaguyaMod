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
using Kaguya.HinaMods;
using Kaguya.HinaMods.Cards;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.HinaMods.Cards.Uncommon;

public sealed class HinaModsFortuneExpend : HinaModsCard
{
	// 保留：使用后弃置
	public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

	// 保留：悬浮提示
	protected override IEnumerable<IHoverTip> GetCustomHoverTips()
	{
		return new IHoverTip[]
		{
			HoverTipFactory.FromPower<BufferPower>(),
			HoverTipFactory.FromPower<FortuneGuardPower>(),
			HoverTipFactory.FromPower<FortunePower>(),
		};
	}

	// 基础费用：3费（保持不变）
	public HinaModsFortuneExpend()
		: base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		// 修复：补全 choiceContext + 正确来源卡牌 this
		await PowerCmd.Apply<BufferPower>(
			choiceContext,
			Owner.Creature,
			1m,
			Owner.Creature,
			this);

		await PowerCmd.Apply<FortuneGuardPower>(
			choiceContext,
			Owner.Creature,
			1m,
			Owner.Creature,
			this);
	}

	// 升级：费用-1
	protected override void OnUpgrade()
	{
		EnergyCost.UpgradeBy(-1);
	}
}
