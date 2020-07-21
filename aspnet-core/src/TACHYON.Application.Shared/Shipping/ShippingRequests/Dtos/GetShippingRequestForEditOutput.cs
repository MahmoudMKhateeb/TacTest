using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class GetShippingRequestForEditOutput
    {
		public CreateOrEditShippingRequestDto ShippingRequest { get; set; }

		public string TrucksTypeDisplayName { get; set;}

		public string TrailerTypeDisplayName { get; set;}

		public string GoodsDetailName { get; set;}

		public string RouteDisplayName { get; set;}


    }
}