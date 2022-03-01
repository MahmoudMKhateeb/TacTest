
using System;
using Abp.Application.Services.Dto;

namespace TACHYON.UnitOfMeasures.Dtos
{
    public class UnitOfMeasureDto : EntityDto
    {
		public string DisplayName { get; set; }
        public bool IsOther { get; set; }



    }
}