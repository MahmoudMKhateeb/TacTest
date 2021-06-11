using Abp.Application.Services.Dto;

namespace TACHYON.PriceOffers.Dto
{
    public class ShippingRequestForPriceOfferGetAllInput: PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public long? ShippingRequestId { get; set; }
        public PriceOfferChannel Channel { get; set; }
    }
}
