using Abp.Application.Services.Dto;
using System.Collections.Generic;
using TACHYON.DynamicInvoices.DynamicInvoiceItems;

namespace TACHYON.DynamicInvoices.Dto
{
    public class DynamicInvoiceForViewDto : EntityDto<long>
    {
        public string CreditCompany { get; set; }
        
        public string DebitCompany { get; set; }

        public long WaybillNumber { get; set; }

        public List<DynamicInvoiceItemDto> Items { get; set; }
    }
}