using Abp.Application.Services.Dto;

namespace TACHYON.MultiTenancy.TenantCarriers.Dto
{
    public class TenantCarriersListDto:EntityDto<long>
    {
        public string CarrierName { get; set; }
    }
}
