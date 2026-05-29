// 严格匹配你的命名空间
using Kaguya.HinaMods.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using Kaguya.HinaMods.Cards;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
// 🔥 引用 BaseLib 的 CustomPowerModel
using BaseLib.Abstracts;
using System.Threading.Tasks;

namespace Kaguya.HinaMods.Powers;

// 🔥 核心：直接继承官方TemporaryStrengthPower（全逻辑复用）
public sealed class TempStrengthDown : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<HinaModsMoonLeverage>();

    protected override bool IsPositive => false;
}