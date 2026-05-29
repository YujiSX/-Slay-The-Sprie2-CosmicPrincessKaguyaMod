using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class SeaSlugBlessing : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;

        private const int MaxHpGain = 16;
        private const int EnergyGain = 1;

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new MaxHpVar(MaxHpGain),
            new EnergyVar(EnergyGain)
        };

        protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
        {
            HoverTipFactory.ForEnergy(this)
        };

        public override string PackedIconPath => "res://images/relics/sea_slug_blessing.png";
        protected override string PackedIconOutlinePath => "res://images/relics/sea_slug_blessing.png";
        protected override string BigIconPath => "res://images/relics/sea_slug_blessing.png";

        // 拾起时增加最大生命值
        public override async Task AfterObtained()
        {
            await CreatureCmd.GainMaxHp(Owner.Creature, MaxHpGain);
        }

        // 增加最大能量上限（每回合开始时的能量+1）
        public override decimal ModifyMaxEnergy(Player player, decimal amount)
        {
            if (player != Owner) return amount;
            return amount + EnergyGain;
        }
    }
}