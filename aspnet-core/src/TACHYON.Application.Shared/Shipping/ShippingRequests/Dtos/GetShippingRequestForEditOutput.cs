using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class GetShippingRequestForEditOutput
    {
        public CreateOrEditShippingRequestDto ShippingRequest { get; set; }
    }
}