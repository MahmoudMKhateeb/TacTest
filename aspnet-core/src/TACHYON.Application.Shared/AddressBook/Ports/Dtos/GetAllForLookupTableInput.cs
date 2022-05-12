using Abp.Application.Services.Dto;

namespace TACHYON.AddressBook.Ports.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}