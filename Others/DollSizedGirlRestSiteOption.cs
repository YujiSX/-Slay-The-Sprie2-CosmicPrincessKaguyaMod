using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using Kaguya.Characters;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    public sealed class DollSizedGirlRestSiteOption : RestSiteOption
    {
        private const int GoldCost = 80;
        private readonly RelicModel _sourceRelic;
        private bool _isEnabled;
        public override bool IsEnabled => _isEnabled;

        public override string OptionId => "DOLL_SIZED_GIRL_ADOPT";

        public override LocString Description
        {
            get
            {
                var loc = new LocString("rest_site_ui", $"OPTION_{OptionId}.description");
                if (_isEnabled)
                {
                    loc.Add("Gold", GoldCost);
                    return loc;
                }
                return new LocString("rest_site_ui", $"OPTION_{OptionId}.descriptionDisabled");
            }
        }

        public DollSizedGirlRestSiteOption(Player owner, RelicModel sourceRelic) : base(owner)
        {
            _sourceRelic = sourceRelic;
            _isEnabled = owner.Gold >= GoldCost;
        }

        public override async Task<bool> OnSelect()
        {
            if (!_isEnabled) return false;

            // 扣除金币
            await PlayerCmd.LoseGold(GoldCost, Owner);

            // 从牌组中选择一张牌进行变化
            var prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1);
            var selected = await CardSelectCmd.FromDeckForTransformation(prefs: prefs, player: Owner);
            var cardToTransform = selected.FirstOrDefault();
            if (cardToTransform != null)
            {
                // 获取所有伙伴牌（带有标�?004�?
                var sakayoriIroha = ModelDb.Character<SakayoriIroha>();
                if (sakayoriIroha == null) return false;
                var allCards = sakayoriIroha.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint);
                var partnerCards = allCards.Where(c => c.Tags.Contains((CardTag)1004)).ToList();
                if (partnerCards.Count > 0)
                {
                    // 随机选择一张伙伴牌
                    var randomPartner = Owner.RunState.Rng.Niche.NextItem(partnerCards);
                    // 创建升级后的副本
                    var upgradedPartner = Owner.RunState.CreateCard(randomPartner, Owner);
                    CardCmd.Upgrade(upgradedPartner);
                    // 执行变化：移除原牌，添加新牌
                    await CardPileCmd.RemoveFromDeck(cardToTransform);
                    await CardPileCmd.Add(upgradedPartner, PileType.Deck);
                }
            }

            // 替换遗物：移除当前遗物，添加“超时空辉夜姬�?
            await RelicCmd.Remove(_sourceRelic);
            var newRelic = ModelDb.Relic<SuperdimensionalKaguya>().ToMutable();
            await RelicCmd.Obtain(newRelic, Owner);

            // 返回true表示成功，休息处界面会关�?
            return true;
        }
    }
}