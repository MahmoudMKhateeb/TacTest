using Abp.Application.Services.Dto;
using System;

namespace TACHYON.AddressBook.Dtos
{
    public class GetAllFacilitiesForExcelInput
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