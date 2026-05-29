using BaseLib.Abstracts;
using BaseLib.Utils;
using Kaguya.RelicPools;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.TestSupport;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaguya.Relics
{
    [Pool(typeof(KaguyaRelicPool))]
    public sealed class SunHeaddress : CustomRelicModel
    {
        private const int MaxCardsToPlay = 168;

        public override RelicRarity Rarity => RelicRarity.Ancient;

        protected override IEnumerable<DynamicVar> CanonicalVars => new[]
        {
            new EnergyVar(2)   // 修改为 2 点能量
        };

        protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
        {
            HoverTipFactory.ForEnergy(this)
        };

        public override string PackedIconPath => "res://images/relics/sun_headdress.png";
        protected override string PackedIconOutlinePath => "res://images/relics/sun_headdress_outline.png";
        protected override string BigIconPath => "res://images/relics/sun_headdress_big.png";

        // 永久增加最大能量上限（每回合开始时会获得该上限值）
        public override decimal ModifyMaxEnergy(Player player, decimal amount)
        {
            if (player != Owner) return amount;
            return amount + DynamicVars.Energy.BaseValue;
        }

        // 自动打牌阶段（与月亮头饰完全一致，每回合触发）
        public override async Task AfterAutoPrePlayPhaseEnteredLate(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner) return;
            ICombatState combatState = player.Creature.CombatState;
            Flash();

            int cardsPlayed = 0;
            using (CardSelectCmd.PushSelector(new SunCardSelector()))
            {
                while (cardsPlayed < MaxCardsToPlay)
                {
                    if (CombatManager.Instance.IsOverOrEnding) break;
                    var hand = PileType.Hand.GetPile(Owner).Cards.ToList();
                    if (hand.Count == 0) break;

                    bool played = false;

                    var partnerCard = hand.FirstOrDefault(c => c.Tags.Contains((CardTag)1004) && c.CanPlay());
                    if (partnerCard != null)
                    {
                        var matchingNonPartner = hand.FirstOrDefault(c =>
                            !c.Tags.Contains((CardTag)1004) &&
                            c.Type == partnerCard.Type &&
                            c.CanPlay());
                        if (matchingNonPartner != null)
                        {
                            await PlayCard(choiceContext, matchingNonPartner, combatState);
                            cardsPlayed++;
                            played = true;
                            if (cardsPlayed < MaxCardsToPlay && !CombatManager.Instance.IsOverOrEnding)
                            {
                                var freshPartner = PileType.Hand.GetPile(Owner).Cards
                                    .FirstOrDefault(c => c == partnerCard || (c.Tags.Contains((CardTag)1004) && c.CanPlay()));
                                if (freshPartner != null)
                                {
                                    await PlayCard(choiceContext, freshPartner, combatState);
                                    cardsPlayed++;
                                    played = true;
                                }
                            }
                            if (played) continue;
                        }
                    }

                    var attackCard = hand.FirstOrDefault(c => c.Type == CardType.Attack && c.CanPlay());
                    if (attackCard != null)
                    {
                        await PlayCard(choiceContext, attackCard, combatState);
                        cardsPlayed++;
                        played = true;
                        continue;
                    }

                    var anyCard = hand.FirstOrDefault(c => c.CanPlay());
                    if (anyCard != null)
                    {
                        await PlayCard(choiceContext, anyCard, combatState);
                        cardsPlayed++;
                        played = true;
                        continue;
                    }

                    if (!played) break;
                }
            }

            if (cardsPlayed > 0)
            {
                bool isFull = cardsPlayed >= MaxCardsToPlay;
                LocString line = isFull
                    ? new LocString("relics", "SUN_HEADDRESS.warning")
                    : new LocString("relics", "SUN_HEADDRESS.approval");
                TalkCmd.Play(line, Owner.Creature, VfxColor.Purple);
            }
        }

        private async Task PlayCard(PlayerChoiceContext choiceContext, CardModel card, ICombatState combatState)
        {
            if (!card.CanPlay()) return;
            if (CombatManager.Instance.IsOverOrEnding) return;
            Creature target = GetTarget(card, combatState);
            await card.SpendResources();
            await CardCmd.AutoPlay(choiceContext, card, target, AutoPlayType.Default, skipXCapture: true);
        }

        private Creature GetTarget(CardModel card, ICombatState combatState)
        {
            Rng combatTargets = Owner.RunState.Rng.CombatTargets;
            return card.TargetType switch
            {
                TargetType.AnyEnemy => combatState.HittableEnemies.FirstOrDefault(),
                TargetType.AnyAlly => combatTargets.NextItem(combatState.Allies.Where(c => c != null && c.IsAlive && c.IsPlayer && c != Owner.Creature)),
                TargetType.AnyPlayer => Owner.Creature,
                _ => null,
            };
        }
    }

    public class SunCardSelector : ICardSelector
    {
        public Task<IEnumerable<CardModel>> GetSelectedCards(IEnumerable<CardModel> options, int minSelect, int maxSelect)
        {
            return Task.FromResult(options.Take(maxSelect));
        }

        public CardRewardSelection GetSelectedCardReward(IReadOnlyList<CardCreationResult> options, IReadOnlyList<CardRewardAlternative> alternatives)
        {
            // 自动打牌阶段不会调用此方法，返回默认结构体即可
            return default(CardRewardSelection);
        }

        public Task<Creature> SelectTarget(PlayerChoiceContext context, TargetType targetType, IEnumerable<Creature> validTargets, CardModel card)
        {
            return Task.FromResult(validTargets.FirstOrDefault());
        }
    }
}