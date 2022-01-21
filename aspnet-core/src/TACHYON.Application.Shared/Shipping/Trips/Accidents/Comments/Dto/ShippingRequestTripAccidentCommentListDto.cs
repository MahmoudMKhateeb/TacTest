using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Shipping.Trips.Accidents.Dto
{
    public class ShippingRequestTripAccidentCommentListDto : EntityDto
    {
        public string Comment { get; set; }
        public DateTime CreationTime { get; set; }
        public long CreatorUserId { get; set; }
        public long TenantId { get; set; }
        public string TenantName { get; set; }
        public string TenantImage { get; set; }
    }
}