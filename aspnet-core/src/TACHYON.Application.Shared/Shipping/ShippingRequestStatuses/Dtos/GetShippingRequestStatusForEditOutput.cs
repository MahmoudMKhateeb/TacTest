using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Shipping.ShippingRequestStatuses.Dtos
{
    public class GetShippingRequestStatusForEditOutput
    {
		public CreateOrEditShippingRequestStatusDto ShippingRequestStatus { get; set; }


    }
}