using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.PlateTypes.Dtos
{
    public class GetPlateTypeForEditOutput
    {
        public CreateOrEditPlateTypeDto PlateType { get; set; }
    }
}