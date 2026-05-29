using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class Demoralize : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseDraw = 1;
    private const int vulnerableAmount = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(baseDraw),
        new PowerVar<VulnerablePower>(vulnerableAmount)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<VulnerablePower>()
    };

    public Demoralize() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 给予自身一层易伤（补充 choiceContext）
        await PowerCmd.Apply<VulnerablePower>(choiceContext, Owner.Creature, DynamicVars["VulnerablePower"].BaseValue, Owner.Creature, this);

        int consumeCount = IsUpgraded ? 2 : 1;
        int drawAmount = (int)DynamicVars["Cards"].BaseValue;

        var drawPile = PileType.Draw.GetPile(Owner);
        var drawCards = drawPile.Cards.ToList();
        if (drawCards.Count > 0)
        {
            int actualConsume = System.Math.Min(consumeCount, drawCards.Count);
            var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, actualConsume);
            var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, drawCards, Owner, prefs);
            foreach (var card in selected)
            {
                if (card != null)
                {
                    await CardCmd.Exhaust(choiceContext, card);
                }
            }
        }

        await CardPileCmd.Draw(choiceContext, drawAmount, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Cards"].UpgradeValueBy(1);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(Demoralize)}.png";
}
