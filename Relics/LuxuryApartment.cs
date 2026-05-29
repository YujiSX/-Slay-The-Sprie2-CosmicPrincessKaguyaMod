using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using Kaguya.Powers;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.GameInfo.Objects;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class LuxuryApartment : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Starter;

        protected override IEnumerable<DynamicVar> CanonicalVars => new[]
        {
            new PowerVar<CreationPower>(2)
        };

        public override string PackedIconPath => $"res://images/relics/LuxuryApartmenticon.png";
        protected override string PackedIconOutlinePath => $"res://images/relics/LuxuryApartmenticonoutline.png";
        protected override string BigIconPath => $"res://images/relics/LuxuryApartment.png";

        protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
        {
            HoverTipFactory.FromKeyword(RealityKeyword.Reality),
            HoverTipFactory.FromPower<Overwork>(),
            HoverTipFactory.FromPower<CreationPower>()
        };

        // 进入战斗房间时触发：进入现实
        public override async Task AfterRoomEntered(AbstractRoom room)
        {
            if (room is CombatRoom)
            {
                Flash();
                await PowerCmd.Apply<Reality>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1, Owner.Creature, null);
            }
        }

        // 每回合开始时触发：获得2层创作（替代 BeforeHandDraw）
        public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
        {
            if (side != CombatSide.Player) return;
            var player = Owner;
            if (player == null) return;

            Flash();
            await PowerCmd.Apply<CreationPower>(new ThrowingPlayerChoiceContext(), player.Creature, 2, player.Creature, null);
        }

        // 每场战斗开始时：获得4层创作
        public override async Task BeforeCombatStart()
        {
            Flash();
            await PowerCmd.Apply<CreationPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 4, Owner.Creature, null);
        }
    }
}
