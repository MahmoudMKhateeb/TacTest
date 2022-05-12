using Abp.Application.Services.Dto;

namespace TACHYON.Shipping.Drivers.Dto
{
    public class ShippingRequestTripDriverRoutPointRatingDto : EntityDto<long>
    {
        public int Rate { get; set; }
        public string ReceiverNote { get; set; }
    }
}