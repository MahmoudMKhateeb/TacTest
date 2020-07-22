using Abp.Application.Services.Dto;

namespace TACHYON.Goods.GoodCategories.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}