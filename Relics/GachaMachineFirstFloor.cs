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
    public sealed class GashaponMachineL1 : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;

        // 遗物描述中动态显示“2”个遗物
        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new DynamicVar("Relics", 2m)
        };

        // 悬停提示：显示“保底”诅咒卡牌
        protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
        {
            HoverTipFactory.FromCard<Guarantee>()
        };

        public override string PackedIconPath => "res://images/relics/gashapon_machine_l1.png";
        protected override string PackedIconOutlinePath => "res://images/relics/gashapon_machine_l1_outline.png";
        protected override string BigIconPath => "res://images/relics/gashapon_machine_l1_big.png";

        public override bool HasUponPickupEffect => true;

        private static IEnumerable<RelicModel> GetValidRelics(IRunState state)
        {
            return (from o in ModelDb.Event<Neow>().AllPossibleOptions
                    where o.Relic != null && o.Relic.IsAllowed(state) && !(o.Relic is GashaponMachineL1)
                    select o.Relic).OfType<RelicModel>();
        }

        public override async Task AfterObtained()
        {
            var availableRelics = GetValidRelics(Owner.RunState).ToList();
            if (availableRelics.Count == 0) return;

            // 与超级扭蛋机完全相同的随机选取方式
            var rng = Owner.PlayerRng.Rewards;
            var selectedRelics = availableRelics.OrderBy(_ => rng.NextFloat())
                                                .Take((int)DynamicVars["Relics"].BaseValue)
                                                .ToList();

            // 奖励选择（自动处理多人同步）
            var rewards = selectedRelics.Select(r => (Reward)new RelicReward(r, Owner)).ToList();
            await new RewardsSet(Owner).WithCustomRewards(rewards).WithSkippingDisallowed().Offer();

            // 添加诅咒卡牌并弹出预览（保留本地/远程区分，确保多人同步）
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