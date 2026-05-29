using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.CardPools;
using Kaguya.Cards;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class GashaponMachineL3 : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;
        public override bool HasUponPickupEffect => true;

        // 动态显示遗物数量
        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new DynamicVar("Relics", 2m)
        };

        // 悬停预览诅咒卡牌
        protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
        {
            HoverTipFactory.FromCard<Guarantee>()
        };

        public override string PackedIconPath => "res://images/relics/gashapon_machine_l3.png";
        protected override string PackedIconOutlinePath => "res://images/relics/gashapon_machine_l3_outline.png";
        protected override string BigIconPath => "res://images/relics/gashapon_machine_l3_big.png";

        private static IEnumerable<RelicModel> GetValidRelics(IRunState state)
        {
            var nonupeipeOptions = ModelDb.Event<Nonupeipe>().AllPossibleOptions
                .Where(o => o.Relic != null && o.Relic.IsAllowed(state) && !(o.Relic is GashaponMachineL3));
            var tanxOptions = ModelDb.Event<Tanx>().AllPossibleOptions
                .Where(o => o.Relic != null && o.Relic.IsAllowed(state) && !(o.Relic is GashaponMachineL3));
            var vakuuOptions = ModelDb.Event<Vakuu>().AllPossibleOptions
                .Where(o => o.Relic != null && o.Relic.IsAllowed(state) && !(o.Relic is GashaponMachineL3));

            return nonupeipeOptions.Concat(tanxOptions).Concat(vakuuOptions)
                .Select(o => o.Relic).OfType<RelicModel>();
        }

        public override async Task AfterObtained()
        {
            var availableRelics = GetValidRelics(Owner.RunState).ToList();
            if (availableRelics.Count == 0) return;

            // 完全对齐超级扭蛋机的随机选取方式
            var rng = Owner.PlayerRng.Rewards;
            var selectedRelics = availableRelics.OrderBy(_ => rng.NextFloat()).Take(2).ToList();

            // 奖励选择（自动处理多人同步）
            var rewards = selectedRelics.Select(r => (Reward)new RelicReward(r, Owner)).ToList();
            await new RewardsSet(Owner).WithCustomRewards(rewards).WithSkippingDisallowed().Offer();

            // 添加“保底”诅咒卡牌，仅本地处理（无需手动对齐随机数）
            if (LocalContext.IsMe(Owner))
            {
                var guaranteeCard = Owner.RunState.CreateCard<Guarantee>(Owner);
                var addResult = await CardPileCmd.Add(guaranteeCard, PileType.Deck);
                RunManager.Instance.RewardSynchronizer.SyncLocalObtainedCard(addResult.cardAdded);
                CardCmd.PreviewCardPileAdd(new[] { addResult }, 2f);
                await Cmd.Wait(0.75f);
            }
        }
    }
}