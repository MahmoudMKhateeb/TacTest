using System;
using Abp.Application.Services.Dto;

namespace TACHYON.TermsAndConditions.Dtos
{
    public class TermAndConditionTranslationDto : EntityDto
    {
        public string Language { get; set; }

        public int? CoreId { get; set; }
    }
}