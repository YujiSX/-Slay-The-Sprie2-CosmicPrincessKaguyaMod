using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.Cards;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class MemoryFragmentResolution : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;

        private const int HpGain = 16;

        // 用于描述显示
        protected override IEnumerable<DynamicVar> CanonicalVars => new[]
        {
            new MaxHpVar(HpGain)
        };

        // 显示决心卡牌和记忆选项关键词
        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
            HoverTipFactory.FromCardWithCardHoverTips<Determination>();

        public override string PackedIconPath => "res://images/relics/memory_fragment_resolution.png";
        protected override string PackedIconOutlinePath => "res://images/relics/memory_fragment_resolution.png";
        protected override string BigIconPath => "res://images/relics/memory_fragment_resolution.png";

        // 标记遗物有拾起时效果
        public override bool HasUponPickupEffect => true;

        // 拾起时：增加16最大生命值，并加入一张“决心”
        public override async Task AfterObtained()
        {
            // 增加最大生命值
            NDebugAudioManager.Instance.Play("NoBadEnd.mp3");
            await CreatureCmd.GainMaxHp(Owner.Creature, HpGain);

            // 创建并添加决心卡牌
            var card = Owner.RunState.CreateCard<Determination>(Owner);
            if (card != null)
            {
                var addResult = await CardPileCmd.Add(card, PileType.Deck);
                CardCmd.PreviewCardPileAdd(new[] { addResult }, 2f);
            }
        }
    }
}
