using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Vases.Dtos
{
    public class GetAvailableVasesInputDto : PagedAndSortedResultRequestDto
    {
        [Required]
        public int CarrierTenantId { get; set; }
    }
}