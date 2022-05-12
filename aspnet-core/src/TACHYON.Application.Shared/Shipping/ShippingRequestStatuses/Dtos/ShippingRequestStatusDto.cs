using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Shipping.ShippingRequestStatuses.Dtos
{
    public class ShippingRequestStatusDto : EntityDto
    {
        public string DisplayName { get; set; }
    }
}