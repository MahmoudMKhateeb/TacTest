using Abp.Application.Services.Dto;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.Shipping.DirectRequests.Dto
{
    public class ShippingRequestDirectRequestListDto : EntityDto<long>
    {
        public string Carrier { get; set; }

        /// <summary>
        /// Carrier Id
        /// </summary>
        public int TenantId { get; set; }
        public decimal CarrierRate { get; set; }
        public int CarrierRateNumber { get; set; }
        public ShippingRequestDirectRequestStatus Status { get; set; }
        public string StatusTitle { get { return Status.GetEnumDescription(); } }
        public string RejetcReason { get; set; }
    }
}