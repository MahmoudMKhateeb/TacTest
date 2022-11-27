using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Regions.Dtos
{
    public class GetAllRegionsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string NameFilter { get; set; }

        public int? MaxBayanIntegrationIdFilter { get; set; }
        public int? MinBayanIntegrationIdFilter { get; set; }

        public string CountyDisplayNameFilter { get; set; }

    }
}