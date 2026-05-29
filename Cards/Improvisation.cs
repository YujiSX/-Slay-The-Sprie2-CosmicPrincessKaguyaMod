using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class Improvisation : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseCreation = 3;

    // 显式指定键名 "CreationPower"，便于访问
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<CreationPower>("CreationPower", baseCreation)
    };

    // 虚无关键词
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Ethereal };

    // 悬浮提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<CreationPower>(),
        HoverTipFactory.FromKeyword(MemoryKeyword.Memory) // 添加回忆关键词提示
    };

    public Improvisation() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得创作（补充 choiceContext）
        int amount = (int)DynamicVars["CreationPower"].BaseValue;
        await PowerCmd.Apply<CreationPower>(choiceContext, Owner.Creature, amount, Owner.Creature, this);

        // 从抽牌堆选择一张牌“回忆”
        var drawPile = PileType.Draw.GetPile(Owner);
        var drawCards = drawPile.Cards.ToList();
        if (drawCards.Count > 0)
        {
            var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
            var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, drawCards, Owner, prefs);
            var card = selected.FirstOrDefault();
            if (card != null)
            {
                await ProcessRecall(card, choiceContext);
            }
        }
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
                // 补充 choiceContext
                await PowerCmd.Apply<RecallPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
                recallPower = Owner.Creature.GetPower<RecallPower>();
            }
            recallPower?.AddCard(card);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：创作层数 +1（3 → 4）
        DynamicVars["CreationPower"].UpgradeValueBy(1);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(Improvisation)}.png";
}
