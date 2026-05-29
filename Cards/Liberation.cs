using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards;

[Pool(typeof(KaguyaCardPool))]
public class Liberation : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    private const int baseEnergy = 1;
    private const int baseDraw = 1;

    // 基础版本具有消耗关键词
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new EnergyVar(baseEnergy),
        new CardsVar(baseDraw)
    };

    public Liberation() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var creature = Owner.Creature;
        var debuffs = creature.Powers.Where(p => p.Type == PowerType.Debuff).ToList();
        int count = debuffs.Count;

        foreach (var debuff in debuffs)
        {
            await PowerCmd.Remove(debuff);
        }

        if (count > 0)
        {
            // 每移除一种负面状态，抽1张牌并获得1点能量
            int energyAmount = (int)DynamicVars["Energy"].BaseValue;
            int drawAmount = (int)DynamicVars["Cards"].BaseValue;
            await PlayerCmd.GainEnergy(energyAmount * count, Owner);
            await CardPileCmd.Draw(choiceContext, drawAmount * count, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后添加保留关键词（消耗关键词仍然保留）
        AddKeyword(CardKeyword.Retain);
    }

    public override string PortraitPath => $"res://images/packed/card_portraits/kaguya/{nameof(Liberation)}.png";
}
