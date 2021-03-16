using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Common;

namespace TACHYON.Invoices.Periods.Dto
{
   public class PeriodCommonDto
    {
        public IEnumerable<KeyValuePair> Weeks { get; set;  }
        public IEnumerable<KeyValuePair> Months { get; set; }

    }
}
