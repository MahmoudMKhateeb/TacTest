
using System;
using Abp.Application.Services.Dto;

namespace TACHYON.AddressBook.Dtos
{
    public class FacilityDto : EntityDto<long>
    {
		public string Name { get; set; }

		public string Adress { get; set; }

		public decimal Longitude { get; set; }

		public decimal Latitude { get; set; }


		 public int CountyId { get; set; }

		 		 public int CityId { get; set; }

		 
    }
}