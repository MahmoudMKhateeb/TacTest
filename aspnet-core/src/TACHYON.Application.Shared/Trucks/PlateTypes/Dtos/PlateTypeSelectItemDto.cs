using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Dto;

namespace TACHYON.Trucks.PlateTypes.Dtos
{
    public class PlateTypeSelectItemDto : SelectItemDto, ISelectItemDto
    {
        public bool IsDefault { get; set; }
    }
}