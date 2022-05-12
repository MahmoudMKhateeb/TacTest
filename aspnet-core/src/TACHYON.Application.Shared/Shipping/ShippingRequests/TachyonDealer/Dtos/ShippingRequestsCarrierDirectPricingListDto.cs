using Abp.Application.Services.Dto;

namespace TACHYON.Shipping.ShippingRequests.TachyonDealer.Dtos
{
    public class ShippingRequestsCarrierDirectPricingListDto : EntityDto
    {
        public long RequestId { get; set; }
        public string CarrierName { get; set; }
        public string ShipperName { get; set; }
        public decimal? Price { get; set; }
        public ShippingRequestsCarrierDirectPricingStatus Status { get; set; }
        public string StatusTitle { get; set; }
        public string RejetcReason { get; set; }
    }
}