
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trucks.Dtos
{
    public class TruckStatusDto : EntityDto<long>
    {
        public string DisplayName { get; set; }



    }
}