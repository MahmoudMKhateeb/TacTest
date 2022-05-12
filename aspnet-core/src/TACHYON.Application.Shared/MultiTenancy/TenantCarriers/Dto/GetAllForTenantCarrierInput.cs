using Abp.Application.Services.Dto;

namespace TACHYON.MultiTenancy.TenantCarriers.Dto
{
    public class GetAllForTenantCarrierInput : PagedAndSortedResultRequestDto
    {
        public int Id { get; set; }
    }
}