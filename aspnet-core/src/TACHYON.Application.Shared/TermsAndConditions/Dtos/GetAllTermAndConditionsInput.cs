using Abp.Application.Services.Dto;
using System;

namespace TACHYON.TermsAndConditions.Dtos
{
    public class GetAllTermAndConditionsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string TitleFilter { get; set; }

        public string ContentFilter { get; set; }

        public double? MaxVersionFilter { get; set; }
        public double? MinVersionFilter { get; set; }

        public int? MaxEditionIdFilter { get; set; }
        public int? MinEditionIdFilter { get; set; }
    }
}