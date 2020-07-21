
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class CreateOrEditShippingRequestDto : EntityDto<long?>
    {

		public decimal Vas { get; set; }
		
		
		 public Guid? TrucksTypeId { get; set; }
		 
		 		 public int? TrailerTypeId { get; set; }
		 
		 		 public long? GoodsDetailId { get; set; }
		 
		 		 public int? RouteId { get; set; }
		 
		 
    }
}