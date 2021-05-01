using Abp.Application.Services.Dto;

namespace TACHYON.Localization.Dto
{
    public  class AppLocalizationFilterInput: PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public int? EditionId { get; set; }
        public string Page { get; set; }

    }
}
