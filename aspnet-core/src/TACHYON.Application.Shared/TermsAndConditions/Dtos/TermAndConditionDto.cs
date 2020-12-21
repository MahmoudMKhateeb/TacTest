
using System;
using Abp.Application.Services.Dto;

namespace TACHYON.TermsAndConditions.Dtos
{
    public class TermAndConditionDto : EntityDto
    {
		public string Title { get; set; }

		public string Content { get; set; }

		public double Version { get; set; }

		public int? EditionId { get; set; }
		public string EditionName { get; set; }
		public bool IsActive { get; set; }



    }
}