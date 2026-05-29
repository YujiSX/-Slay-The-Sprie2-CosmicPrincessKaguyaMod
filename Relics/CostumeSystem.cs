using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class CostumeSystem : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;

        public override bool HasUponPickupEffect => true;

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new CardsVar(1)
        };

        protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
        {
            HoverTipFactory.Static(StaticHoverTip.Transform)
        };

        public override string PackedIconPath => "res://images/relics/costume_system.png";
        protected override string PackedIconOutlinePath => "res://images/relics/costume_system_outline.png";
        protected override string BigIconPath => "res://images/relics/costume_system_big.png";

        public override async Task AfterObtained()
        {
            // 失去所有金币
            int currentGold = Owner.Gold;
            if (currentGold > 0)
            {
                await PlayerCmd.LoseGold(currentGold, Owner);
            }

            // 选择一张牌变化（随机变化）
            var transformList = await CardSelectCmd.FromDeckForTransformation(
                prefs: new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, (int)DynamicVars["Cards"].BaseValue),
                player: Owner);
            if (transformList != null && transformList.Any())
            {
                var cardToTransform = transformList.First();
                await CardCmd.TransformToRandom(cardToTransform, Owner.RunState.Rng.Niche);
            }

            // 选择一张牌升级
            var upgradeList = await CardSelectCmd.FromDeckForUpgrade(
                prefs: new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, (int)DynamicVars["Cards"].BaseValue),
                player: Owner);
            if (upgradeList != null && upgradeList.Any())
            {
                var cardToUpgrade = upgradeList.First();
                CardCmd.Upgrade(cardToUpgrade);
            }
        }
    }
}