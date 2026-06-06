using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Kaguya
{
    public class PartnerKeyword
    {
        [CustomEnum("PARTNER")]
        [KeywordProperties(AutoKeywordPosition.After)]
        public static CardKeyword Partner;
    }
}