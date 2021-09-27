using Abp.Application.Services.Dto;
using System.Collections.Generic;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Authorization.Users.Profile.Dto
{
    public class FleetInformationDto
    {
        public int TotalDrivers { get; set; }

        public PagedResultDto<TruckTypeAvailableTrucksDto> AvailableTrucksDto { get; set; }
    }
}