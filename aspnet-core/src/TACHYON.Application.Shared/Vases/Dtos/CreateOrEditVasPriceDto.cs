
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Vases.Dtos
{
    public class CreateOrEditVasPriceDto : EntityDto<int?>
    {

		public double? Price { get; set; }
		
		
		public int? MaxAmount { get; set; }
		
		
		public int? MaxCount { get; set; }
		
		
		 public int VasId { get; set; }
		 
		 
    }
}