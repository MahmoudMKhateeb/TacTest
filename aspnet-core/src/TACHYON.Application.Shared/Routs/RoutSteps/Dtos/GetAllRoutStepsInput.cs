using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Routs.RoutSteps.Dtos
{
    public class GetAllRoutStepsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string DisplayNameFilter { get; set; }

        public string LatitudeFilter { get; set; }

        public string LongitudeFilter { get; set; }

        public int? MaxOrderFilter { get; set; }
        public int? MinOrderFilter { get; set; }

        public string CityDisplayNameFilter { get; set; }

        public string CityDisplayName2Filter { get; set; }


        public string TrucksTypeDisplayNameFilter { get; set; }

        public string TrailerTypeDisplayNameFilter { get; set; }

       // public string GoodsDetailNameFilter { get; set; }

    }
}