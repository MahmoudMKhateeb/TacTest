using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.TermsAndConditions.Dtos
{
    public class GetTermAndConditionTranslationForEditOutput
    {
        public CreateOrEditTermAndConditionTranslationDto TermAndConditionTranslation { get; set; }

        public string TermAndConditionTitle { get; set; }

    }
}