using Abp.Application.Services.Dto;

namespace TACHYON.Shipping.DirectRequests.Dto
{
    public class ShippingRequestDirectRequestGetAllCarrirerInput: PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public long ShippingRequestId { get; set; }
    }
}
