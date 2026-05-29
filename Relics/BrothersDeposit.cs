using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    public sealed class BrothersDeposit : RelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;

        public override bool HasUponPickupEffect => true;

        protected override IEnumerable<DynamicVar> CanonicalVars => new[]
        {
            new GoldVar(680)
        };

        public override async Task AfterObtained()
        {
            await PlayerCmd.GainGold(DynamicVars.Gold.BaseValue, Owner);
        }
    }
}