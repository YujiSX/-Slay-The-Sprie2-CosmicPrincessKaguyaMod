using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.Powers;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class WorkClothes : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Uncommon;

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new PowerVar<Overwork>("Overwork", 1m)
        };

        protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
        {
            HoverTipFactory.FromPower<Reality>(),
            HoverTipFactory.FromPower<Overwork>()
        };

        public override string PackedIconPath => "res://images/relics/gongzuofu.png";
        protected override string PackedIconOutlinePath => "res://images/relics/gongzuofu.png";
        protected override string BigIconPath => "res://images/relics/gongzuofu.png";

        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side != CombatSide.Player) return;
            if (Owner == null) return;
            if (Owner.Creature.GetPower<Reality>() == null) return;

            var overwork = Owner.Creature.GetPower<Overwork>();
            if (overwork == null || overwork.Amount <= 0) return;

            Flash();
            var blockVar = new BlockVar((int)overwork.Amount, ValueProp.Move);
            await CreatureCmd.GainBlock(Owner.Creature, blockVar, null);
        }
    }
}