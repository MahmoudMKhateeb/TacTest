using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;

namespace TACHYON.Shipping.ShippingRequestBids.Dtos
{
    public class GetAllBidsShippingRequestForCarrierInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public bool IsMatchingOnly { get; set; }
        public bool IsMyBidsOnly { get; set; }
        public long? TruckTypeId { get; set; }
        public long? TruckSubTypeId { get; set; }
        public long? TransportType { get; set; }
        public long? TransportSubType { get; set; }
        public int? CapacityId { get; set; }
    }


    public class GetAllBidsShippingRequestForShipperInput : PagedAndSortedResultRequestDto
    {

    }
}
