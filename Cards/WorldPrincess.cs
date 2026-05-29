using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Kaguya.Cards
{
    public sealed class WorldPrincess : CardModel
    {
        public override IEnumerable<CardTag> Tags => new[] { (CardTag)1001 };

        public WorldPrincess() : base(2, CardType.Power, CardRarity.Ancient, TargetType.Self) { }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 1. 给予一层人工制品
            await PowerCmd.Apply<ArtifactPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);

            // 2. 施加公主之力能力（层数为1，用于显示触发次数）
            await PowerCmd.Apply<PrincessPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            // 升级后添加固有关键词
            AddKeyword(CardKeyword.Innate);
            // 注意：升级后仍然消耗（未移除消耗关键词）
        }
    }
}
