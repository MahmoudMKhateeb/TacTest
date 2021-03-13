using System.Collections.Generic;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Shipping.ShippingRequestBids.Dtos;
using TACHYON.ShippingRequestVases.Dtos;
using TACHYON.Trucks.Dtos;
using TACHYON.Vases.Dtos;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class GetShippingRequestForViewOutput
    {
        public ShippingRequestDto ShippingRequest { get; set; }

       // public List<ShippingRequestBidDto> ShippingRequestBidDtoList { get; set; }
        public List<GetShippingRequestVasForViewDto> ShippingRequestVasDtoList { get; set; }
        //public List<RoutPointDto> RoutPointDtoList { get; set; }
        public int VasCount { get; set; }
        public string TruckTypeDisplayName { get; set; }
        public string TransportTypeDisplayName { get; set; }
        public string CapacityDisplayName { get; set; }
        public string DriverName { get; set; }
        public string RoutTypeName { get; set; }
        public string OriginalCityName { get; set; }
        public string DestinationCityName { get; set; }
        public string GoodsCategoryName { get; set; }
        public string TruckTypeFullName { get; set; }
        public string ShippingRequestStatusName { get; set; }
        public string packingTypeDisplayName { get; set; }
        public string ShippingTypeDisplayName { get; set; }
        public bool HasTrips { get; set; }
        public GetTruckForViewOutput AssignedTruckDto { get; set; }

    }
}