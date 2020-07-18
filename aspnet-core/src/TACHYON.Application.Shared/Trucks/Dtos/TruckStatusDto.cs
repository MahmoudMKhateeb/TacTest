
using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.Dtos
{
    public class TruckStatusDto : EntityDto<Guid>
    {
		public string DisplayName { get; set; }



    }
}