using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trucks.Dtos
{
    public class GetAllTrucksInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string PlateNumberFilter { get; set; }

        public string ModelNameFilter { get; set; }

        public string ModelYearFilter { get; set; }

        //public string LicenseNumberFilter { get; set; }

        //public DateTime? MaxLicenseExpirationDateFilter { get; set; }
        //public DateTime? MinLicenseExpirationDateFilter { get; set; }

        public int IsAttachableFilter { get; set; }


        public string TrucksTypeDisplayNameFilter { get; set; }

        public string TruckStatusDisplayNameFilter { get; set; }

        //public string UserNameFilter { get; set; }

        //public string UserName2Filter { get; set; }


    }
}