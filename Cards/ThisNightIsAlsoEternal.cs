using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards;

// 临时能力：回合结束时进入现实
public sealed class ThisNightIsAlsoEternalTempPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomPackedIconPath => "res://images/packed/card_portraits/kaguya/ThisNightIsAlsoEternalTempPower.png";
    public override string CustomBigIconPath => "res://images/packed/card_portraits/kaguya/ThisNightIsAlsoEternalTempPower.png";

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Apply<Reality>(choiceContext, Owner, 1, Owner, null);
            await PowerCmd.Remove(this);
        }
    }
}

[Pool(typeof(KaguyaCardPool))]
public class ThisNightIsAlsoEternal : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseScry = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("ScryAmount", baseScry)
    };

    public ThisNightIsAlsoEternal() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromKeyword(Yujian.yujian),
        HoverTipFactory.FromKeyword(MemoryKeyword.Memory),
        HoverTipFactory.FromKeyword(RealityKeyword.Reality)
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 预见
        int scryAmount = (int)DynamicVars["ScryAmount"].BaseValue;
        await ScryHelper.Scry(Owner, scryAmount, choiceContext);

        // 2. 从弃牌堆选择一张牌“回忆”
        var discardPile = PileType.Discard.GetPile(Owner);
        var discardCards = discardPile.Cards.ToList();
        if (discardCards.Count > 0)
        {
            var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
            var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, discardCards, Owner, prefs);
            var card = selected.FirstOrDefault();
            if (card != null)
            {
                await ProcessRecall(card, choiceContext);
            }
        }

        // 3. 添加临时能力：回合结束时进入现实
        await PowerCmd.Apply<ThisNightIsAlsoEternalTempPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }

    private async Task ProcessRecall(CardModel card, PlayerChoiceContext choiceContext)
    {
        // 升级卡牌
        CardCmd.Upgrade(card);
        // 消耗卡牌
        await CardCmd.Exhaust(choiceContext, card);

        // 只有成功升级的卡牌才加入召回列表
        if (card.IsUpgraded)
        {
            var recallPower = Owner.Creature.GetPower<RecallPower>();
            if (recallPower == null)
            {
                await PowerCmd.Apply<RecallPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
                recallPower = Owner.Creature.GetPower<RecallPower>();
            }
            recallPower?.AddCard(card);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：预见数 +3 (3 → 6)
        DynamicVars["ScryAmount"].UpgradeValueBy(3);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(ThisNightIsAlsoEternal)}.png";
}
