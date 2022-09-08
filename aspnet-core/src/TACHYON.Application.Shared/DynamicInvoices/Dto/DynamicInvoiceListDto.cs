using Abp.Application.Services.Dto;

namespace TACHYON.DynamicInvoices.Dto
{
    public class DynamicInvoiceListDto : EntityDto<long>
    {
        public string CreditCompanyName { get; set; }

        public string DebitCompanyName { get; set; }

        public long? InvoiceNumber { get; set; }

        public string Notes { get; set; }
        
    }
}