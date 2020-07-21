using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class GetAllShippingRequestsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public decimal? MaxVasFilter { get; set; }
		public decimal? MinVasFilter { get; set; }


		 public string TrucksTypeDisplayNameFilter { get; set; }

		 		 public string TrailerTypeDisplayNameFilter { get; set; }

		 		 public string GoodsDetailNameFilter { get; set; }

		 		 public string RouteDisplayNameFilter { get; set; }

		 
    }
}