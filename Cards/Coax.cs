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
public class Coax : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseDraw = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(baseDraw)
    };

    public Coax() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int totalDraw = (int)DynamicVars.Cards.BaseValue;

        // 收集抽牌堆和弃牌堆中的诅咒牌（最多 totalDraw 张）
        var drawPile = PileType.Draw.GetPile(Owner);
        var discardPile = PileType.Discard.GetPile(Owner);

        var curses = drawPile.Cards
            .Where(c => c.Type == CardType.Curse)
            .Concat(discardPile.Cards.Where(c => c.Type == CardType.Curse))
            .Take(totalDraw)
            .ToList();

        // 将诅咒牌加入手牌
        foreach (var curse in curses)
        {
            await CardPileCmd.Add(curse, PileType.Hand);
        }

        // 剩余抽牌数量
        int remaining = totalDraw - curses.Count;
        if (remaining > 0)
        {
            await CardPileCmd.Draw(choiceContext, remaining, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后抽牌数 +1
        DynamicVars.Cards.UpgradeValueBy(1);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(Coax)}.png";
}