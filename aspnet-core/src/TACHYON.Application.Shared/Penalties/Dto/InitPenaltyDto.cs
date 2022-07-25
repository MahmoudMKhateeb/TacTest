using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Penalties.Dto
{
   public class InitPenaltyDto
    {
        public string PenaltyName { get; set; }
        public string PenaltyDescrption { get; set; }
        public decimal Amount { get; set; }
        public PenaltyType Type { get; set; }
        public int TenantId { get; set; }
        public long? SourceId { get; set; }
    }
}
