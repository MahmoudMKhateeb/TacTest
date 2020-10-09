
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.TruckCategories.TruckSubtypes.Dtos
{
    public class CreateOrEditTruckSubtypeDto : EntityDto<int?>
    {

		[Required]
		[StringLength(TruckSubtypeConsts.MaxDisplayNameLength, MinimumLength = TruckSubtypeConsts.MinDisplayNameLength)]
		public string DisplayName { get; set; }
		
		
		 public long? TrucksTypeId { get; set; }
		 
		 
    }
}