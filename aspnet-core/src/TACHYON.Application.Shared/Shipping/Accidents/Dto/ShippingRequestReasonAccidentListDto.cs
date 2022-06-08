using Abp.Application.Services.Dto;

namespace TACHYON.Shipping.Accidents.Dto
{
    public class ShippingRequestReasonAccidentListDto : FullAuditedEntityDto
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public bool IsOther { get; set; }

    }
}