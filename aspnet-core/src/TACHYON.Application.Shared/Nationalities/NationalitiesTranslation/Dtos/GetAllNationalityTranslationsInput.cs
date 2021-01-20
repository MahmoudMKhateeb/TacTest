using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Nationalities.NationalitiesTranslation.Dtos
{
    public class GetAllNationalityTranslationsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string TranslatedNameFilter { get; set; }

        public string LanguageFilter { get; set; }

        public string NationalityNameFilter { get; set; }

        public int? NationalityIdFilter { get; set; }
    }
}