
using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.TruckCategories.TransportSubtypes.Dtos
{
    public class TransportSubtypeDto : EntityDto
    {
		public string DisplayName { get; set; }


		 public int? TransportTypeId { get; set; }

		 
    }
}