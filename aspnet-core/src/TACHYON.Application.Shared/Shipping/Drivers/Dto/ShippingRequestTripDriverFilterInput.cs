using Abp.Application.Services.Dto;
using TACHYON.Shipping.Trips;

namespace TACHYON.Shipping.Drivers.Dto
{
    public class ShippingRequestTripDriverFilterInput : PagedAndSortedResultRequestDto
    {
        public ShippingRequestTripStatus? Status { get; set; }
    }
}
