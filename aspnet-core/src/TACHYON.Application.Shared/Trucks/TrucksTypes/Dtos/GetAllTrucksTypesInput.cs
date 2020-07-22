using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trucks.TrucksTypes.Dtos
{
    public class GetAllTrucksTypesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string DisplayNameFilter { get; set; }



    }
}