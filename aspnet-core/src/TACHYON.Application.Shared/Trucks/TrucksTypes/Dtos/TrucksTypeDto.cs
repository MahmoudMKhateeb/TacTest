
using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.TrucksTypes.Dtos
{
    public class TrucksTypeDto : EntityDto<Guid>
    {
		public string DisplayName { get; set; }



    }
}