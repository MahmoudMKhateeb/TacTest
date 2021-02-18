
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.AddressBook.Ports.Dtos
{
    public class CreateOrEditPortDto : EntityDto<long?>
    {

		[Required]
		[StringLength(PortConsts.MaxNameLength, MinimumLength = PortConsts.MinNameLength)]
		public string Name { get; set; }
		
		
		[StringLength(PortConsts.MaxAdressLength, MinimumLength = PortConsts.MinAdressLength)]
		public string Address { get; set; }
		
		
		public decimal Longitude { get; set; }
		
		
		public decimal Latitude { get; set; }
		
		
		 public int CityId { get; set; }
		 
		 
    }
}