using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.TermsAndConditions.Dtos
{
    public class GetTermAndConditionForEditOutput
    {
		public CreateOrEditTermAndConditionDto TermAndCondition { get; set; }


    }
}