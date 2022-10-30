using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class DedicatedTruckDto: EntityDto<long>
    {
        /// <summary>
        /// This field in helper from front to be used in validation, if one truck is busy return the name
        /// </summary>
        public string TruckName { get; set; }
    }
}
