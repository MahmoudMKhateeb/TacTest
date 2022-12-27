using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class DedicatedShippingRequestTrucksDto: EntityDto<long>
    {
        public string TruckType { get; set; }
        public string Status { get; set; }
        public string PlateNumber { get; set; }
        public string Capacity { get; set; }
        public string ShippingRequestReference { get; set; }
        public string CarrierName { get; set; }
        public string Duration { get; set; }
        public double? KPI { get; set; }
        public int NumberOfTrips { get; set; }

    }
}
