using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaguya
{
    public class AdoptKeyword
    {
        [CustomEnum("ADOPT")]
        [KeywordProperties(AutoKeywordPosition.Before)]
        public static CardKeyword Adopt;
    }
}
