using Abp.Application.Services.Dto;
using System;

namespace TACHYON.DynamicInvoices.DynamicInvoiceItems
{
    public class DynamicInvoiceItemDto : EntityDto<long>
    {
        public string Description { get; set; }

        public decimal Price { get; set; }
        
        public long? WaybillNumber { get; set; }

        public int? OriginCityId { get; set; }

        public int? DestinationCityId { get; set; }

        public long? TruckTypeId { get; set; }

        public DateTime? WorkDate { get; set; }

        public int? Quantity { get; set; }

        public string PlateNumber { get; set; }
        
        public string ContainerNumber { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }


    }
}