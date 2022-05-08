using System;

namespace TACHYON.Common.Dto
{
    public class GetAllTranslationInput<TCoreId> : LoadOptionsInput
        where TCoreId : IComparable
    {
        public TCoreId CoreId { get; set; }
    }
}