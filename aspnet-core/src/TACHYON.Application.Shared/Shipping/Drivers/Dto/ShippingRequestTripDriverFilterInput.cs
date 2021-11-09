using Abp.Application.Services.Dto;
using TACHYON.Shipping.Trips;

namespace TACHYON.Shipping.Drivers.Dto
{
    public class ShippingRequestTripDriverFilterInput : PagedAndSortedResultRequestDto
    {
        public ShippingRequestTripDriverLoadStatusDto? Status { get; set; }
    }


    public enum ShippingRequestTripDriverLoadStatusDto
    {
        Current = 0,
        Comming = 1,
        Past = 2
    }

}