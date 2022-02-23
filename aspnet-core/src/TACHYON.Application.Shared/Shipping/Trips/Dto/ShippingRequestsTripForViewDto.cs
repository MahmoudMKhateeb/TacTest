using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.ShippingRequestTripVases.Dtos;

namespace TACHYON.Shipping.Trips.Dto
{
    public class ShippingRequestsTripForViewDto
    {

        public DateTime StartTripDate { get; set; }
        public DateTime? EndTripDate { get; set; }
        public DateTime? StartWorking { get; set; }
        public DateTime? EndWorking { get; set; }
        public string StatusTitle { get; set; }
        public string RoutePointStatus { get; set; }
        public long? AssignedDriverUserId { get; set; }
        public long? AssignedTruckId { get; set; }
        public bool HasAttachment { get; set; }
        public bool NeedsDeliveryNote { get; set; }
        public string Driver { get; set; }
        public string Truck { get; set; }
        public string OriginFacility { get; set; }
        public string DestinationFacility { get; set; }
        public DocumentFileDto DocumentFile { get; set; }

        public ICollection<RoutPointDto> RoutPoints { get; set; }
        public ICollection<ShippingRequestTripVasDto> ShippingRequestTripVases { get; set; }

        public ShippingRequestTripDriverStatus DriverStatus { get; set; }
        public string DriverStatusTitle { get; set; }
        public string RejectedReason { get; set; }
        public string TotalValue { get; set; }
        public string Note { get; set; }
        
        public long? WaybillNumber { get; set; }

        public ShippingRequestTripStatus Status { get; set; }
    }
}
