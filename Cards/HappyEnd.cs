using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class HappyEnd : CustomCardModel
{
    private const int initialRequiredEnergy = 168;
    private int _currentRequiredEnergy = initialRequiredEnergy;
    private bool _hasAppliedInitialReduction = false;

    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("RequiredEnergy", initialRequiredEnergy)
    };

    public HappyEnd() : base(0, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override bool IsPlayable
    {
        get
        {
            if (!base.IsPlayable) return false;
            return Owner.PlayerCombatState.Energy >= _currentRequiredEnergy;
        }
    }

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    private void UpdateRequiredEnergy(int newValue)
    {
        if (newValue < 0) newValue = 0;
        _currentRequiredEnergy = newValue;
        DynamicVars["RequiredEnergy"].BaseValue = newValue;
    }

    public override async Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this) return;
        if (_hasAppliedInitialReduction) return;
        if (IsClone) return;

        int cardsPlayed = CombatManager.Instance.History.CardPlaysFinished.Count(e => e.CardPlay.Card.Owner == Owner);
        if (cardsPlayed > 0)
        {
            UpdateRequiredEnergy(_currentRequiredEnergy - cardsPlayed);
        }
        _hasAppliedInitialReduction = true;
        await Task.CompletedTask;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner) return;
        if (cardPlay.Card == this) return;
        if (_currentRequiredEnergy > 0)
        {
            UpdateRequiredEnergy(_currentRequiredEnergy - 1);
        }
        await Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 阻止自动打出
        if (cardPlay.IsAutoPlay) return;

        // GrandFinale 特效
        NGrandFinaleVfx nGrandFinaleVfx = NGrandFinaleVfx.Create(Owner.Creature);
        if (nGrandFinaleVfx != null)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nGrandFinaleVfx);
            await Cmd.Wait(NGrandFinaleVfx.totalAnticipationDuration);
        }

        // 完全模拟 win 指令逻辑：获取所有敌人（包括无敌、隐藏的）
        var enemies = Owner.Creature.CombatState.Enemies.ToList();
        foreach (var enemy in enemies)
        {
            enemy.RemoveAllPowersInternalExcept(); // 内部移除所有能力，绕过转阶段保护
            await CreatureCmd.Kill(enemy);
        }

        await CombatManager.Instance.CheckWinCondition();
    }

    // 被消耗时返回手牌
    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (!ReferenceEquals(card, this)) return;
        await CardPileCmd.Add(this, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(HappyEnd)}.png";
}