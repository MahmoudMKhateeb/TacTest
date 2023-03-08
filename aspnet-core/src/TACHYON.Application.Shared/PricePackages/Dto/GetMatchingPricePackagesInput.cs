using TACHYON.Dto;

namespace TACHYON.PricePackages.Dto
{
    public class GetMatchingPricePackagesInput: PagedInputDto
    {
        public long ShippingRequestId { get; set; }
    }
}