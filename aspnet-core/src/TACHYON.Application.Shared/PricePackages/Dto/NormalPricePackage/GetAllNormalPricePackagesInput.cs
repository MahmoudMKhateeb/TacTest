using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.PricePackages.Dto.NormalPricePackage
{
    public class GetAllNormalPricePackagesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public string DestinationFilter { get; set; }
        public string OriginFilter { get; set; }
        public string TruckTypeFilter { get; set; }
        public string PricePackageIdFilter { get; set; }
    }
}