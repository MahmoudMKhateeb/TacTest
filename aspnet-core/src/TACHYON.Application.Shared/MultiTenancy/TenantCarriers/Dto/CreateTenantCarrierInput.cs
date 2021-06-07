using Abp.Application.Services.Dto;

namespace TACHYON.MultiTenancy.TenantCarriers.Dto
{
    public class CreateTenantCarrierInput:EntityDto<int>
    {
        public int CarrierTenantId { get; set; }
    }
}
