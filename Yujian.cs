using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaguya
{
    public class Yujian
    {
        // 自定义枚举的名字。最终会变成{前缀}-{枚举值大写}的形式，例如TEST-UNIQUE
        [CustomEnum("YUJIAN")]
        // 放在原版卡牌描述的位置，这里是卡牌描述的前面
        [KeywordProperties(AutoKeywordPosition.Before)]
        public static CardKeyword yujian;
    }
}