using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Shipping.Trips.Dto
{
    public class UpdateExpectedDeliveryTimeInput : EntityDto
    {
        public DateTime? ExpectedDeliveryTime { get; set; }
        
    }
}