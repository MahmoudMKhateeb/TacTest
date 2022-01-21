using Abp.Application.Services.Dto;

namespace TACHYON.AddressBook.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}