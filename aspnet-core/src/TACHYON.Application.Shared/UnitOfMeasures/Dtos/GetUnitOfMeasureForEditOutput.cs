using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.UnitOfMeasures.Dtos
{
    public class GetUnitOfMeasureForEditOutput
    {
        public CreateOrEditUnitOfMeasureDto UnitOfMeasure { get; set; }
    }
}