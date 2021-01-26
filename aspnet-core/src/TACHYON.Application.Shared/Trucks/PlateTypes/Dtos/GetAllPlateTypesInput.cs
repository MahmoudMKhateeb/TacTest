using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trucks.PlateTypes.Dtos
{
    public class GetAllPlateTypesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string DisplayNameFilter { get; set; }

    }
}