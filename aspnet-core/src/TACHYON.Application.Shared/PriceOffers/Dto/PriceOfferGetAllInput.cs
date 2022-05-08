using Abp.Application.Services.Dto;

namespace TACHYON.PriceOffers.Dto
{
    public class PriceOfferGetAllInput : PagedAndSortedResultRequestDto
    {
        public long id { get; set; }
        public PriceOfferChannel? Channel { get; set; }
    }
}