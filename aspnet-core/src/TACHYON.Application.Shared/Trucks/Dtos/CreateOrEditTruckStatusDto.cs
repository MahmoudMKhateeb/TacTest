
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.Dtos
{
    public class CreateOrEditTruckStatusDto : EntityDto<Guid?>
    {

		[Required]
		[StringLength(TruckStatusConsts.MaxDisplayNameLength, MinimumLength = TruckStatusConsts.MinDisplayNameLength)]
		public string DisplayName { get; set; }
		
		

    }
}