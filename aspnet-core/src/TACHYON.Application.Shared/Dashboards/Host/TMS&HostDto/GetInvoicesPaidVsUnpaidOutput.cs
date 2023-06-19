using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Dashboards.Shipper.Dto;

namespace TACHYON.Dashboards.Host.TMS_HostDto
{
    public class GetInvoicesPaidVsUnpaidOutput
    {
        public List<ChartCategoryPairedValuesDto> PaidInvoices { get; set; }
        public List<ChartCategoryPairedValuesDto> UnPaidInvoices { get; set; }
    }
}
