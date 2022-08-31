using Abp.Application.Services.Dto;

namespace TACHYON.DynamicInvoices.DynamicInvoiceItems
{
    public class DynamicInvoiceItemDto : EntityDto<long>
    {
        public string Description { get; set; }

        public decimal Price { get; set; }
    }
}