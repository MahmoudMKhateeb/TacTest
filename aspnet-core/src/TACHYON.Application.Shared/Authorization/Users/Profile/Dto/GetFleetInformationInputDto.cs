using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Authorization.Users.Profile.Dto
{
    public class GetFleetInformationInputDto : PagedAndSortedResultRequestDto
    {
        [Required] public int TenantId { get; set; }
    }
}