
using System;
using Abp.Application.Services.Dto;

namespace TACHYON.AddressBook.Ports.Dtos
{
    public class PortDto : EntityDto<long>
    {
		public string Name { get; set; }

		public string Adress { get; set; }

		public decimal Longitude { get; set; }

		public decimal Latitude { get; set; }


		 public int CityId { get; set; }

		 
    }
}