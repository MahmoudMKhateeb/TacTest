using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.Dedicated;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class GetDedicatedDriverForReplaceDto: EntityDto<long>
    {
        public string DriverName { get; set; }
        public bool IsRequestedToReplace { get; set; }
        public DateTime? ReplacementDate { get; set; }
        public string ReplacementReason { get; set; }
        public long? OriginalDriverId { get; set; }
        public string OriginalDriverName { get; set; }
        public int? ReplacementIntervalInDays { get; set; }
        public ReplacementFlag ReplacementFlag { get; set; }
    }
}
