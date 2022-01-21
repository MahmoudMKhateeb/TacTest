using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trucks.TruckCategories.TruckCapacities.Dtos
{
    public class GetAllCapacitiesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string DisplayNameFilter { get; set; }


        public string TruckTypeDisplayNameFilter { get; set; }
    }
}