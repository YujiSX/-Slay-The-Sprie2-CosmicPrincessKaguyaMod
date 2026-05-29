using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Kaguya.HinaMods;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.HinaMods.Cards;

/// <summary>
/// 歌者专属：等待
/// 效果：选择{Cards:diff()}张消耗堆中的【歌者】牌，加入手牌并获得保留
/// </summary>
public sealed class SingerWaitBlock : HinaModsCard
{
    // 官方标准动态变量：CardsVar
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    // 1费 技能 稀有
    public SingerWaitBlock()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    { }

    // 歌者标签
    public override HashSet<string> CustomTags => new HashSet<string>() { CustomCardTags.SINGER };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Player player = Owner;
        if (player == null) return;

        // 完全对标官方动画写法
        await CreatureCmd.TriggerAnim(player.Creature, "Cast", player.Character.CastAnimDelay);

        // 筛选消耗堆歌者牌
        CardPile exhaustPile = PileType.Exhaust.GetPile(player);
        List<CardModel> singerCards = exhaustPile.Cards
            .OfType<HinaModsCard>()
            .Where(c => c.CustomTags?.Contains(CustomCardTags.SINGER) == true)
            .Cast<CardModel>()
            .ToList();

        if (!singerCards.Any())
            return;

        int selectCount = DynamicVars.Cards.IntValue;

        // 🔥 官方标准写法：使用卡牌自身的本地化提示 + 0~N选择（防卡死）
        // 对标 Nightmare 源码，Min=0，Max=动态数值
        CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 0, selectCount);

        IEnumerable<CardModel> selectedCards = await CardSelectCmd.FromSimpleGrid(
            context: choiceContext,
            cardsIn: singerCards,
            player: player,
            prefs: prefs
        );

        // 移入手牌 + 保留
        foreach (CardModel card in selectedCards)
        {
            await CardPileCmd.Add(card, PileType.Hand);
            CardCmd.ApplyKeyword(card, CardKeyword.Retain);
        }
    }

    // 升级：数量+1
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}