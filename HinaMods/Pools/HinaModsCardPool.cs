using BaseLib.Abstracts;
using Godot;
using Kaguya.HinaMods.Cards.Rare;
using Kaguya.HinaMods.Cards.Uncommon;
using Kaguya.HinaMods.Powers;
using Kaguya.HinaMods.SupportCards.Moons;
using Kaguya.HinaMods.SupportCards.Common;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Kaguya.HinaMods.Cards;
using Kaguya.HinaMods.Extensions;
using Kaguya.HinaModsCode.cards.Rare;

namespace Kaguya.HinaMods.Character;

public class HinaModsCardPool : CustomCardPoolModel
{
	public override string CardFrameMaterialPath => "hina";
	// 卡池ID（绑定角色）
	public override string Title => HinaCharacter.CharacterId;

	public override Texture2D CustomFrame(CustomCardModel card)
	{
		return card.Type switch
		{
			CardType.Attack => PreloadManager.Cache.GetAsset<Texture2D>("res://scenes/hinamods/cards/card_frame_attack.png"),
			CardType.Power => PreloadManager.Cache.GetAsset<Texture2D>("res://scenes/hinamods/cards/card_frame_power.png"),
			_ => PreloadManager.Cache.GetAsset<Texture2D>("res://scenes/hinamods/cards/card_frame_skill.png"),
		};
	}
	// 能量图标路径
	public override string BigEnergyIconPath => "Charui/big_energy.png".ImagePath();
	public override string TextEnergyIconPath => "Charui/text_energy.png".ImagePath();

	public override Color DeckEntryCardColor => new("#f79431");
	// 非无色卡牌（角色专属）
	public override bool IsColorless => false;

	protected override CardModel[] GenerateAllCards()
	{
		return new CardModel[]
		{
			// 基础卡牌
			ModelDb.Card<GenerateSupportCard>(),
			ModelDb.Card<GenerateSupportStrike>(),
			ModelDb.Card<HinaModsMoonBarrier>(),
			ModelDb.Card<HinaModsMoonEclipse>(),
			ModelDb.Card<HinaModsMoonEclipseStrike>(),
			ModelDb.Card<HinaModsMoonScar>(),
			ModelDb.Card<HinaModsMuffin>(),
			ModelDb.Card<MoonTideReturn>(),
			ModelDb.Card<SingerAllStrike>(),
			ModelDb.Card<SingerGuard>(),
			ModelDb.Card<SingerSelfDebuff>(),
			ModelDb.Card<SingerStrike>(),
			//ModelDb.Card<HinaModsLiveAccident>(),
			ModelDb.Card<HinaModsClearGame>(),
			//ModelDb.Card<HinaModsMakeup>(),
			ModelDb.Card<HinaModsOmurice>(),
			ModelDb.Card<HinaModsBirth>(),
			ModelDb.Card<DailySong>(),
			ModelDb.Card<SingerHoldLine>(),
			ModelDb.Card<SingerRecycle>(),
			ModelDb.Card<MoonlightTripleStrike>(),
			ModelDb.Card<FullMoonStrike>(),
			ModelDb.Card<SupportAllocation>(),
			ModelDb.Card<HinaModsMoonFinalStrike>(),

		// 罕见卡牌
			ModelDb.Card<SingerDraw>(),
			ModelDb.Card<HinaModsMoonDraw>(),
			ModelDb.Card<HinaModsFortuneExpend>(),
			ModelDb.Card<HinaModsBreakEvil>(),
			ModelDb.Card<ExhaustSingerGuard>(),
			ModelDb.Card<NoteCombo>(),
			ModelDb.Card<HinaModsMoonConcerto>(),
			ModelDb.Card<SupportStrikeAuraCard>(),
			ModelDb.Card<HomeRun>(),
			ModelDb.Card<Growth>(),
			ModelDb.Card<SingerSupportGuard>(),
			ModelDb.Card<SupportBurst>(),
			ModelDb.Card<SingerBulwark>(),
			ModelDb.Card<ParryParry>(),
			ModelDb.Card<TsukimiDraw>(),
			ModelDb.Card<TsukimiFrenzy>(),
			ModelDb.Card<GameLife>(),
			ModelDb.Card<StarNightFree>(),
			ModelDb.Card<SingerFugue>(),
			ModelDb.Card<SingerWaitBlock>(),
			ModelDb.Card<SingerReshape>(),
			ModelDb.Card<MoonLightAria>(),
			ModelDb.Card<SupportRhythmCard>(),
			ModelDb.Card<SupportEnergyCharge>(),
			ModelDb.Card<SupportMoonlightCall>(),
			ModelDb.Card<SingerMoonStrike>(),

			// 稀有卡牌
			ModelDb.Card<SupportAuraCard>(),
			ModelDb.Card<SingerEchoCopy>(),
			ModelDb.Card<TsukimiForm>(),
			ModelDb.Card<Encore>(),
			ModelDb.Card<RallySupportCard>(),
			ModelDb.Card<HeavenlyRaimentCard>(),
			ModelDb.Card<HinaModsLeap>(),
			ModelDb.Card<TsukimiRobe>(),
			ModelDb.Card<SingerConvert>(),
			ModelDb.Card<SingerCascade>(),
			ModelDb.Card<SingerComboStrike>(),
			ModelDb.Card<SupportObliteration>(),
			ModelDb.Card<HinaModsMoonlight>(),

			ModelDb.Card<ConcertedPreparation>(),
			ModelDb.Card<SupportAssemble>(),
			//ModelDb.Card<SupportAssist>(),
			ModelDb.Card<FinalPerformance>(),
  
			//月见卡牌
			//ModelDb.Card<HinaModsTsukimiFinalStrike>(),
			ModelDb.Card<HinaModsTsukimiBlessing>(),
			//ModelDb.Card<HinaModsTsukimiUltimate>(),
			//ModelDb.Card<HinaModsTsukimiPurge>(),

			ModelDb.Card<HinaModsMoonlightRecall>(),
			ModelDb.Card<HinaModsWinStrike>(),
			ModelDb.Card<SingerComb>(),
			ModelDb.Card<LeapStrike>(),
			//ModelDb.Card<DragonRide>(),
			ModelDb.Card<GraduationPerformance>(),
			ModelDb.Card<SupportForgeCard>(),
			ModelDb.Card<MoonlightSlash>(),
			ModelDb.Card<MoonlightOverflow>(),
			ModelDb.Card<Confluence>(),
		   
			//多人联机牌
			ModelDb.Card<MultiSupportGather>(),
			ModelDb.Card<MoonlightTeamBless>(),
		};

	}
}
