using Abp;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.Shipping.Trips.Dto
{
    public class DeliveredTripDto
    {
        public int Id { get; set; }
        public long? ShippingRequestId { get; set; }
        public ShippingRequestRouteType? RequestRouteType { get; set; }
        public ShippingRequestRouteType? RouteType { get; set; }
        public ShippingRequestTripStatus Status { get; set; }
        public UserIdentifier ShipperUser { get; set; }
        public int? TripShipperTenantId { get; set; }
        public int? TripCarrierTenantId { get; set; }

        public int CarrierTenantId { get; set; }
        public InvoiceTripStatus InvoiceTripStatus { get; set; }
        public InvoiceTripStatus CarrierInvoiceTripStatus { get; set; }
    }
}