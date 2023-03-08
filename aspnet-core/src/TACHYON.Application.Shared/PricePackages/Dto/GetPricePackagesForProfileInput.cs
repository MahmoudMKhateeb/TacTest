using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.PricePackages.Dto
{
    public class GetPricePackagesForProfileInput : PagedAndSortedResultRequestDto
    {
        [Required] 
        public int CarrierTenantId { get; set; }
    }
}