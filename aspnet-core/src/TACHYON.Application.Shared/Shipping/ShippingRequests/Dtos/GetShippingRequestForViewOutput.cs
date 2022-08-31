using System.Collections.Generic;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Shipping.ShippingRequestBids.Dtos;
using TACHYON.ShippingRequestVases.Dtos;
using TACHYON.Trucks.Dtos;
using TACHYON.Vases.Dtos;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    /// <summary>
    /// mapped from ShippingRequest
    /// </summary>
    public class GetShippingRequestForViewOutput
    {
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// This reference shipper add it manually
        /// </summary>
        public string ShipperReference { get; set; }

        /// <summary>
        /// shipper add his invoice number manually
        /// </summary>
        public string ShipperInvoiceNo { get; set; }

        public decimal ShipperRating { get; set; }
        public int ShipperRatingNumber { get; set; }
        public ShippingRequestDto ShippingRequest { get; set; }

        public List<ShippingRequestBidDto> ShippingRequestBidDtoList { get; set; }

        public List<GetShippingRequestVasForViewDto> ShippingRequestVasDtoList { get; set; }

        //public List<RoutPointDto> RoutPointDtoList { get; set; }
        public int VasCount { get; set; }
        public string TruckTypeDisplayName { get; set; }
        public long TruckTypeId { get; set; }
        public string TransportTypeDisplayName { get; set; }
        public string CapacityDisplayName { get; set; }
        public string DriverName { get; set; }
        public string RoutTypeName { get; set; }
        public string OriginalCityName { get; set; }
        public int OriginalCityId { get; set; }
        public string DestinationCityName { get; set; }
        //public int DestinationCityId { get; set; }
        public List<ShippingRequestDestinationCitiesDto> DestinationCitiesDtos { get; set; }
        public string GoodsCategoryName { get; set; }
        public string TruckTypeFullName { get; set; }
        public string ShippingRequestStatusName { get; set; }
        public string packingTypeDisplayName { get; set; }
        public string ShippingTypeDisplayName { get; set; }
        public bool HasTrips { get; set; }
        public GetTruckForViewOutput AssignedTruckDto { get; set; }

        public int TotalsTripsAddByShippier { get; set; }

        //this field special to tachyon user
        public decimal? CarrierPrice { get; set; }
        public string CarrierName { get; set; }

        /// <summary>
        /// shipper
        /// </summary>
        public int TenantId { get; set; }
    }
}