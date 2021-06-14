using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.ShippingRequestTripVases.Dtos;

namespace TACHYON.Shipping.Trips.Dto
{
    public class ShippingRequestsTripListDto: EntityDto<long>
    {
        public DateTime StartTripDate { get; set; }
        public DateTime EndTripDate { get; set; }
        public DateTime? StartWorking { get; set; }
        public DateTime? EndWorking { get; set; }
        public ShippingRequestTripStatus Status { get; set; }
        public bool HasAttachment { get; set; }
        public bool NeedsDeliveryNote { get; set; }

        public string StatusTitle 
        { 
            get {
                    return Enum.GetName(typeof(ShippingRequestTripStatus), Status);
                }
        }
        public string Driver { get; set; }
        public string Truck { get; set; }
        public string OriginFacility { get; set; }
        public string DestinationFacility { get; set; }
        public bool HasAccident { get; set; }
        public bool IsApproveCancledByShipper { get; set; }
        public bool IsApproveCancledByCarrier { get; set; }

        public ShippingRequestTripDriverStatus DriverStatus { get; set; }
        public string DriverStatusTitle { get; set; }
        public string RejectedReason { get; set; }

    }
}
