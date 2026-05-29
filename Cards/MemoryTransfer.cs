using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class MemoryTransfer : CustomCardModel
{
    private const int energyCost = 3;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    // 未升级时具有消耗关键词
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public MemoryTransfer() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => System.Array.Empty<DynamicVar>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获取消耗牌堆中的卡牌（排除当前卡牌自身）
        var exhaustPile = PileType.Exhaust.GetPile(Owner);
        var cardsToPlay = exhaustPile.Cards.Where(c => c != this).ToList();

        foreach (var card in cardsToPlay)
        {
            await CardCmd.AutoPlay(choiceContext, card, null);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(MemoryTransfer)}.png";
}