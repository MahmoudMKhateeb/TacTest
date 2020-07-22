
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trucks.Dtos
{
    public class TruckStatusDto : EntityDto<Guid>
    {
        public string DisplayName { get; set; }



    }
}