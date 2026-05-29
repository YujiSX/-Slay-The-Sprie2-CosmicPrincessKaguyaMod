using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.Cards;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class SuperGashaponMachine : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;
        public override bool HasUponPickupEffect => true;

        public override string PackedIconPath => "res://images/relics/gashapon_machine_l1.png";
        protected override string PackedIconOutlinePath => "res://images/relics/gashapon_machine_l1.png";
        protected override string BigIconPath => "res://images/relics/gashapon_machine_l1.png";

        // 自定义遗物候选池（不含 Tanx 遗物）
        private static IEnumerable<RelicModel> CustomRelicPool()
        {
            return new RelicModel[]
            {
                ModelDb.Relic<TouchOfOrobas>(),
                ModelDb.Relic<ArchaicTooth>(),
                ModelDb.Relic<PaelsLegion>(),
                ModelDb.Relic<PaelsBlood>(),
                ModelDb.Relic<PaelsTears>(),
                ModelDb.Relic<PaelsEye>(),
                ModelDb.Relic<NutritiousSoup>(),
                ModelDb.Relic<VeryHotCocoa>(),
                ModelDb.Relic<YummyCookie>(),
                ModelDb.Relic<Storybook>(),
                ModelDb.Relic<SealOfGold>(),
                ModelDb.Relic<PumpkinCandle>(),
                ModelDb.Relic<ToyBox>(),
                ModelDb.Relic<RunicPyramid>(),
                ModelDb.Relic<BeautifulBracelet>(),
                ModelDb.Relic<BrilliantScarf>(),
                ModelDb.Relic<DelicateFrond>(),
                ModelDb.Relic<DiamondDiadem>(),
                ModelDb.Relic<JewelryBox>(),
                ModelDb.Relic<SignetRing>(),
                ModelDb.Relic<LoomingFruit>(),
                ModelDb.Relic<DistinguishedCape>(),
                ModelDb.Relic<LordsParasol>(),
                ModelDb.Relic<JeweledMask>(),
                ModelDb.Relic<ChoicesParadox>()
            };
        }

        // 获取 Tanx 事件中的所有遗物选项
        private static IEnumerable<RelicModel> GetTanxRelics(IRunState state)
        {
            var tanxEvent = ModelDb.Event<Tanx>();
            if (tanxEvent == null) return Enumerable.Empty<RelicModel>();

            var prop = typeof(Tanx).GetProperty("AllPossibleOptions");
            if (prop != null && prop.GetValue(tanxEvent) is IEnumerable<EventOption> options)
            {
                return options
                    .Where(o => o.Relic != null && o.Relic.IsAllowed(state))
                    .Select(o => o.Relic)
                    .Where(r => r != null)
                    .OfType<RelicModel>();
            }
            return Enumerable.Empty<RelicModel>();
        }

        // 合并自定义池与 Tanx 遗物，排除自身
        private static IEnumerable<RelicModel> GetValidRelics(IRunState state)
        {
            var customRelics = CustomRelicPool();
            var tanxRelics = GetTanxRelics(state);

            return customRelics
                .Concat(tanxRelics)
                .Where(r => r != null && !(r is SuperGashaponMachine))
                .Distinct()
                .Where(r => r.IsAllowed(state));
        }

        public override async Task AfterObtained()
        {
            var basePool = GetValidRelics(Owner.RunState).ToList();
            if (basePool.Count == 0) return;

            // 排除玩家已拥有的遗物，防止重复
            var ownedIds = new HashSet<string>(Owner.Relics.Select(r => r.Id.Entry));
            var availableRelics = basePool
                .Where(r => !ownedIds.Contains(r.Id.Entry))
                .ToList();

            if (availableRelics.Count == 0) return;

            // 随机选取三个遗物，并确保是 Mutable 实例
            var rng = Owner.PlayerRng.Rewards;
            var selectedRelics = availableRelics
                .OrderBy(_ => rng.NextFloat())
                .Take(3)
                .Select(r => r.IsMutable ? r : r.ToMutable()) // 关键修复
                .ToList();

            // 构建奖励并给予玩家
            var rewards = selectedRelics
                .Select(r => (Reward)new RelicReward(r, Owner))
                .ToList();
            await new RewardsSet(Owner)
                .WithCustomRewards(rewards)
                .WithSkippingDisallowed()
                .Offer();
        }
    }
}