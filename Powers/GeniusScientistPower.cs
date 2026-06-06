using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Threading.Tasks;

namespace Kaguya.Powers;

public sealed class GeniusScientistPower : CustomPowerModel
{
    private int _cardsPlayed;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomPackedIconPath => "res://images/potions/GeniusScientistPower.png";
    public override string CustomBigIconPath => "res://images/potions/GeniusScientistPower.png";

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner)
            return;

        _cardsPlayed++;
        if (_cardsPlayed >= 4)
        {
            _cardsPlayed = 0;
            await CardPileCmd.Draw(choiceContext, 1, Owner.Player!);
            Flash();
        }
    }
}
