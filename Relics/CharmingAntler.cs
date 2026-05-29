using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class CharmingAntler : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new CardsVar(3)
        };

        protected override IEnumerable<IHoverTip> ExtraHoverTips
        {
            get
            {
                List<IHoverTip> tips = new List<IHoverTip>();
                tips.AddRange(HoverTipFactory.FromCardWithCardHoverTips<Dazed>());
                tips.Add(StunIntent.GetStaticHoverTip());
                return tips;
            }
        }

        public override string PackedIconPath => "res://images/relics/charming_antler.png";
        protected override string PackedIconOutlinePath => "res://images/relics/charming_antler.png";
        protected override string BigIconPath => "res://images/relics/charming_antler.png";

        // 使用新版回合开始钩子（替代 BeforeHandDraw），仅在战斗第一回合玩家侧触发
        public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
        {
            if (side != CombatSide.Player) return;
            // 确保是遗物拥有者的回合
            if (!CombatManager.Instance.IsPartOfPlayerTurn(Owner)) return;
            // 只在第一回合生效
            if (combatState.RoundNumber != 1) return;

            Flash();

            int count = (int)DynamicVars["Cards"].BaseValue;
            List<CardModel> dazedCards = new List<CardModel>();
            for (int i = 0; i < count; i++)
            {
                dazedCards.Add(combatState.CreateCard<Dazed>(Owner));
            }

            // 新版签名：AddGeneratedCardsToCombat(cards, pileType, creator, position)
            var addResults = await CardPileCmd.AddGeneratedCardsToCombat(dazedCards, PileType.Draw, Owner, CardPilePosition.Random);
            CardCmd.PreviewCardPileAdd(addResults, 2f);

            // 击晕所有敌人
            foreach (var enemy in combatState.HittableEnemies)
            {
                await CreatureCmd.Stun(enemy);
            }

            await Cmd.Wait(0.5f);
        }
    }
}
