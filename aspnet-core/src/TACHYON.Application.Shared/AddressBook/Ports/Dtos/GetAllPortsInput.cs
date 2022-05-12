using Abp.Application.Services.Dto;
using System;

namespace TACHYON.AddressBook.Ports.Dtos
{
    public class GetAllPortsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string NameFilter { get; set; }

        public string AdressFilter { get; set; }

        public decimal? MaxLongitudeFilter { get; set; }
        public decimal? MinLongitudeFilter { get; set; }

        public decimal? MaxLatitudeFilter { get; set; }
        public decimal? MinLatitudeFilter { get; set; }


        public string CityDisplayNameFilter { get; set; }
    }
}