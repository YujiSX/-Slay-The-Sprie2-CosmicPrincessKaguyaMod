namespace Kaguya.HinaMods.Potions;

using BaseLib.Abstracts;
using Kaguya.HinaMods.Extensions;

public class HinaModsPotionPool : CustomPotionPoolModel
{
    // 描述中使用的能量图标。大小为24x24。
    public override string BigEnergyIconPath => "Charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "Charui/text_energy.png".ImagePath();

}