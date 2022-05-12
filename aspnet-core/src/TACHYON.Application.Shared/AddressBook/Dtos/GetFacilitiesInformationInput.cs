using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.AddressBook.Dtos
{
    public class GetFacilitiesInformationInput : PagedAndSortedResultRequestDto
    {
        [Required] public int TenantId { get; set; }
    }
}