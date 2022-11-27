using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.Dedicated;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class GetDedicatedTruckForReplaceDto : EntityDto<long>
    {
        public string TruckName { get; set; }
        public bool IsRequestedToReplace { get; set; }
        public DateTime? ReplacementDate { get; set; }
        public string ReplacementReason { get; set; }
        public long? OriginalTruckId { get; set; }
        public string OriginalTruckName { get; set; }
        public int? ReplacementIntervalInDays { get; set; }
        public ReplacementFlag ReplacementFlag { get; set; }
    }
}
