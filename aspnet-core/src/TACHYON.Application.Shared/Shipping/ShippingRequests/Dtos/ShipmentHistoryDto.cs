using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class ShipmentHistoryDto : EntityDto<long>
    {
        public int? CarrierTenantId { get; set; }
        public int ShipperId { get; set; }
        public string ShipperReference { get; set; }
        public string StatusTitle { get; set; }
        public string RequestType { get; set; }
        public string CarrierName { get; set; }
        public string ShipperName { get; set; }

    }
}