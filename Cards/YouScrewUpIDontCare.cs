using BaseLib.Abstracts;
using BaseLib.Utils;
using BaseLib.Audio;
using Kaguya.CardPools;
using Kaguya.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class YouScrewUpIDontCare : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AnyAlly;
    private const bool shouldShowInCardLibrary = true;

    private const int bufferAmount = 1;
    private const int overworkAmount = 3;

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<BufferPower>(bufferAmount),
        new PowerVar<Overwork>(overworkAmount)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<BufferPower>(),
        HoverTipFactory.FromPower<Overwork>()
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public YouScrewUpIDontCare() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null || cardPlay.Target == Owner.Creature)
            return;

        // 播放音效，音量 +3dB（可根据需要调整）
        ModAudio.PlaySound("res://audios/gaozha.ogg", volumeAdd: 12f);

        await PowerCmd.Apply<BufferPower>(choiceContext, cardPlay.Target, bufferAmount, Owner.Creature, this);
        await PowerCmd.Apply<Overwork>(choiceContext, Owner.Creature, overworkAmount, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(YouScrewUpIDontCare)}.png";
}
