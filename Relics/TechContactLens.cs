using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.Powers;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class TechContactLens : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Shop;

        protected override IEnumerable<DynamicVar> CanonicalVars => System.Array.Empty<DynamicVar>();

        public override string PackedIconPath => "res://images/relics/meitong.png";
        protected override string PackedIconOutlinePath => "res://images/relics/meitong.png";
        protected override string BigIconPath => "res://images/relics/meitong.png";

        public override async Task BeforeCombatStart()
        {
            Flash();
            await PowerCmd.Apply<Tsukuyomi>(
                new ThrowingPlayerChoiceContext(),
                Owner.Creature,
                1,
                Owner.Creature,
                null);
        }
    }
}