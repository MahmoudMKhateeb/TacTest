
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.UnitOfMeasures.Dtos
{
    public class UnitOfMeasureDto : EntityDto
    {
        public string Key { get; set; }
        public string DisplayName { get; set; }
        public bool IsOther { get; set; }



    }
}