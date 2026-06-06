using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.Powers;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class SchoolUniform : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Common;

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new CardsVar(1)
        };

        protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
        {
            HoverTipFactory.FromPower<Reality>()
        };

        public override string PackedIconPath => "res://images/relics/xaiofu.png";
        protected override string PackedIconOutlinePath => "res://images/relics/xaiofu.png";
        protected override string BigIconPath => "res://images/relics/xaiofu.png";

        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side != CombatSide.Player) return;
            if (Owner == null) return;
            if (Owner.Creature.GetPower<Reality>() == null) return;

            Flash();
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        }
    }
}