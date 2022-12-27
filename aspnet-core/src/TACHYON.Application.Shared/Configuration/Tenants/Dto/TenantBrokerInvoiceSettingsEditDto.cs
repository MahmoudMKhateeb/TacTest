using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TACHYON.Configuration.Tenants.Dto
{
    public class TenantBrokerInvoiceSettingsEditDto
    {

        public string BrokerBankNameEnglish { get; set; }
        public string BrokerBankNameArabic { get; set; }
        public string BrokerBankAccountNumber { get; set; }
        public string BrokerIban { get; set; }
        public string BrokerEmailAddress { get; set; }
        public string BrokerWebSite { get; set; }
        public string BrokerAddress { get; set; }
        public string BrokerMobile { get; set; }
    }
}
