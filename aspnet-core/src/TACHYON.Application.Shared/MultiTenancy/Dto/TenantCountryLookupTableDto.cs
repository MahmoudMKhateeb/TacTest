using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.MultiTenancy.Dto
{
   public class TenantCountryLookupTableDto
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Code { get; set; }
    }
}
