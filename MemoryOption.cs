using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Kaguya
{
    public class MemoryOptionKeyword
    {
        [CustomEnum("MEMORYOPTION")]
        [KeywordProperties(AutoKeywordPosition.Before)]
        public static CardKeyword Mo;
    }
}