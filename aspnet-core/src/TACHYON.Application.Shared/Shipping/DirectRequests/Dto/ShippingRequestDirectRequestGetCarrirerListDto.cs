using Abp.Application.Services.Dto;

namespace TACHYON.Shipping.DirectRequests.Dto
{
    public class ShippingRequestDirectRequestGetCarrirerListDto : EntityDto
    {
        public string Name { get; set; }
        public bool IsRequestSent { get; set; }
    }
}
