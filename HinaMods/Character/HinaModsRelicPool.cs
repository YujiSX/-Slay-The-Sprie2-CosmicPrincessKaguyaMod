using BaseLib.Abstracts;
using Godot;
using Kaguya.HinaMods.Character;
using System;

namespace Kaguya.HinaMods.Character;

public partial class HinaModsRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => HinaCharacter.CharacterId;

    public override Color LabOutlineColor => HinaCharacter.Color;
}