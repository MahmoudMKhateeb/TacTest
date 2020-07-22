
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trucks.TrucksTypes.Dtos
{
    public class TrucksTypeDto : EntityDto<Guid>
    {
        public string DisplayName { get; set; }



    }
}