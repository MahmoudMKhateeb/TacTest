using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;

namespace TACHYON.AddressBook.Dtos
{
    public class GetAllFacilitiesInput : PagedAndSortedResultRequestDto, IShouldNormalize
    {
        public string Filter { get; set; }

        public string NameFilter { get; set; }

        public string AdressFilter { get; set; }

        public decimal? MaxLongitudeFilter { get; set; }
        public decimal? MinLongitudeFilter { get; set; }

        public decimal? MaxLatitudeFilter { get; set; }
        public decimal? MinLatitudeFilter { get; set; }

        public string CityDisplayNameFilter { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrWhiteSpace(Sorting)) return;
            if (Sorting.Contains("longitude"))
            {
                Sorting = Sorting.Replace("longitude", "Location.X");
            }
            else if (Sorting.Contains("latitude"))
            {
                Sorting = Sorting.Replace("latitude", "Location.Y");
            }
        }
    }
}