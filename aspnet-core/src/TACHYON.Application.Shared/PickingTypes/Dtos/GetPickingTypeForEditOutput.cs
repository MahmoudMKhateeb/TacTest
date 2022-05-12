using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.PickingTypes.Dtos
{
    public class GetPickingTypeForEditOutput
    {
        public CreateOrEditPickingTypeDto PickingType { get; set; }
    }
}