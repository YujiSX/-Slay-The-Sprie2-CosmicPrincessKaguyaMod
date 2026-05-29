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
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.GameInfo.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class RundownApartment : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Starter;

        protected override IEnumerable<DynamicVar> CanonicalVars => System.Array.Empty<DynamicVar>();

        public override string PackedIconPath => $"res://images/relics/rundown_apartment.png";
        protected override string PackedIconOutlinePath => $"res://images/relics/rundown_apartment.png";
        protected override string BigIconPath => $"res://images/relics/rundown_apartment.png";

        protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
        {
            HoverTipFactory.FromPower<Overwork>(),
            HoverTipFactory.FromKeyword(RealityKeyword.Reality),
            HoverTipFactory.FromPower<CreationPower>()
        };

        public override async Task AfterRoomEntered(AbstractRoom room)
        {
            if (room is CombatRoom)
            {
                Flash();
                await PowerCmd.Apply<Reality>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1, Owner.Creature, null);
            }
        }

        // 修正：直接使用 Owner 玩家对象，无需通过 Creature 查找
        public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
        {
            if (side != CombatSide.Player) return;
            // 确保遗物拥有者玩家存在且处于战斗中
            if (Owner == null) return;

            Flash();
            await PowerCmd.Apply<CreationPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1, Owner.Creature, null);
        }

        public override RelicModel GetUpgradeReplacement() => ModelDb.Relic<LuxuryApartment>();
    }
}
