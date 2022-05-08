using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.AddressBook.Dtos;
using TACHYON.Trucks.Dtos;

namespace TACHYON.Shipping.Trips.Dto
{
    public class ShippingRequestTripForViewDto
    {
        public ShippingRequestTripDto shippingRequestTripDto { get; set; }
        public FacilityDto OriginalFacilityDto { get; set; }
        public FacilityDto DestinationFacilityDto { get; set; }
        public ShippingRequestTripStatus TripStatus { get; set; }
        public string AssignedDriverDisplayName { get; set; }
        public GetTruckForViewOutput AssignedTruckDto { get; set; }
    }
}