using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.UnitOfMeasures.Dtos
{
    public class UnitOfmeasureTranslationDto : EntityDto
    {
        public string DisplayName { get; set; }
        public string Language { get; set; }
        public int CoreId { get; set; }
    }
}