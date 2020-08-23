
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trucks.TrucksTypes.Dtos
{
    public class TrucksTypeDto : EntityDto<long>
    {
        public string DisplayName { get; set; }



    }
}