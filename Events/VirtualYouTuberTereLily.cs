using Godot;
using Kaguya.Cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Events
{
    public sealed class VirtualYouTuberTereLily : EventModel
    {
        protected override IEnumerable<DynamicVar> CanonicalVars => System.Array.Empty<DynamicVar>();

        protected override IReadOnlyList<EventOption> GenerateInitialOptions()
        {
            return new[]
            {
                new EventOption(this, HuntingSkill, InitialOptionKey("HUNTING"),
                    HoverTipFactory.FromCardWithCardHoverTips<ElementalArrow>()), // 添加元素箭预览
                new EventOption(this, ElfSecret, InitialOptionKey("ELF"))
            };
        }

        // 选项A：将2张“元素箭”加入牌组
        private async Task HuntingSkill()
        {
            var elementalArrow = Owner.RunState.CreateCard<ElementalArrow>(Owner);
            var elementalArrow2 = Owner.RunState.CreateCard<ElementalArrow>(Owner);
            var addResult1 = await CardPileCmd.Add(elementalArrow, PileType.Deck);
            var addResult2 = await CardPileCmd.Add(elementalArrow2, PileType.Deck);
            CardCmd.PreviewCardPileAdd(new[] { addResult1, addResult2 }, 2f);
            SetEventFinished(L10NLookup("VIRTUAL_YOUTUBER_TERE_LILY.pages.HUNTING.description"));
        }

        // 选项B：选择一张牌升级
        private async Task ElfSecret()
        {
            var upgradePrefs = new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 1);
            var cardsToUpgrade = await CardSelectCmd.FromDeckForUpgrade(prefs: upgradePrefs, player: Owner);
            foreach (var card in cardsToUpgrade)
            {
                CardCmd.Upgrade(card);
            }
            SetEventFinished(L10NLookup("VIRTUAL_YOUTUBER_TERE_LILY.pages.ELF.description"));
        }
    }
}