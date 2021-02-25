using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Packing.PackingTypes.Dtos
{
    public class GetPackingTypeForEditOutput
    {
        public CreateOrEditPackingTypeDto PackingType { get; set; }

    }
}