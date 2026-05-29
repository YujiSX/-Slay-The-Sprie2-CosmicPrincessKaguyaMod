using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.Cards;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class TimelessBond : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;

        private const int MaxHpGain = 25;
        private const int EnergyGain = 1;
        private const int DrawGain = 1;

        private bool _hasSuperKaguya;

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new MaxHpVar(MaxHpGain),
            new EnergyVar(EnergyGain),
            new DynamicVar("ExtraDraw", DrawGain)
        };

        // 同时显示升级版好结局卡牌和超时空辉夜姬遗物
        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
            HoverTipFactory.FromCardWithCardHoverTips<GoodEnding>(upgrade: true)
                .Concat(HoverTipFactory.FromRelic<SuperdimensionalKaguya>());

        public override string PackedIconPath => "res://images/relics/timeless_bond.png";
        protected override string PackedIconOutlinePath => "res://images/relics/timeless_bond.png";
        protected override string BigIconPath => "res://images/relics/timeless_bond.png";

        public override async Task AfterObtained()
        {
            _hasSuperKaguya = Owner.Relics.Any(r => r is EightThousandYearsLonging);
            if (_hasSuperKaguya)
            {
                await CreatureCmd.GainMaxHp(Owner.Creature, MaxHpGain);

                var goodEnding = Owner.RunState.CreateCard<GoodEnding>(Owner);
                if (goodEnding != null)
                {
                    if (goodEnding.IsUpgradable)
                        CardCmd.Upgrade(goodEnding);

                    var addResult = await CardPileCmd.Add(goodEnding, PileType.Deck);
                    CardCmd.PreviewCardPileAdd(new[] { addResult }, 2f);
                }
            }
        }

        public override decimal ModifyMaxEnergy(Player player, decimal amount)
        {
            if (player != Owner) return amount;
            if (_hasSuperKaguya)
                return amount + EnergyGain;
            return amount;
        }

        public override decimal ModifyHandDraw(Player player, decimal count)
        {
            if (player != Owner) return count;
            if (_hasSuperKaguya)
                return count + DrawGain;
            return count;
        }
    }
}
