
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.TruckCategories.TransportSubtypes.Dtos
{
    public class CreateOrEditTransportSubtypeDto : EntityDto<int?>
    {

		[Required]
		[StringLength(TransportSubtypeConsts.MaxDisplayNameLength, MinimumLength = TransportSubtypeConsts.MinDisplayNameLength)]
		public string DisplayName { get; set; }
		
		
		 public int? TransportTypeId { get; set; }
		 
		 
    }
}