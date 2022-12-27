using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.Dedicated;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class DedicatedShippingRequestDriversDto : EntityDto<long>
    {
        public string DriverName { get; set; }
        public string PhoneNumber { get; set; }
        public string AccountNumber { get; set; }
        public string Status { get; set; }
        public string CarrierName { get; set; }
        public string Duration { get; set; }
        public string ShippingRequestReference { get; set; }
        public bool IsRequestedToReplace { get; set; }
        public DateTime? ReplacementDate { get; set; }
        public string ReplacementReason { get; set; }
        public ReplacementFlag ReplacementFlag { get; set; }
        public int ReplacementIntervalInDays { get; set; }
        public long? OriginalDedicatedDriverId { get; set; }
        public string OriginalDedicatedDriverName { get; set; }

    }
}
