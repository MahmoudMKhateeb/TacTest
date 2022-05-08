using Abp.Application.Services.Dto;
using System;

namespace TACHYON.TermsAndConditions.Dtos
{
    public class GetAllTermAndConditionTranslationsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string LanguageFilter { get; set; }

        public string TermAndConditionTitleFilter { get; set; }
    }
}