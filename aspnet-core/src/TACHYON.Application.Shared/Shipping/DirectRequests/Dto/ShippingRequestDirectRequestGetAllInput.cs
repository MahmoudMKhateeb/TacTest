using Abp.Application.Services.Dto;

namespace TACHYON.Shipping.DirectRequests.Dto
{
    public class ShippingRequestDirectRequestGetAllInput: PagedAndSortedResultRequestDto
    {
        public long ShippingRequestId { get; set; }
    }
}
