using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.Trips;
using TACHYON.Shipping.Trips.Accidents.Dto;

namespace TACHYON.Shipping.Drivers.Dto
{
    public class ShippingRequestTripDriverDetailsDto : EntityDto<long>
    {
        public ICollection<ShippingRequestTripDriverRoutePointDto> RoutePoints { get; set; }
        public List<ShippingRequestTripAccidentListDto> ShippingRequestTripAccidentList { get; set; }
        public string StartTripDate { get; set; }
        public string EndTripDate { get; set; }
        public string TravelTime { get; set; }
        public string PackingType { get; set; }
        public string GoodsCategory { get; set; }
        public double TotalWeight { get; set; }
        public string ShipperName { get; set; }
        public decimal ShipperRating { get; set; }
        public bool IsShippingExpRated { get; set; }
        public int ShipperRatingNumber { get; set; }
        public string UnitOfMeasure { get; set; } = "Kg";
        public string Note { get; set; }

        public string Source { get; set; }
        public decimal SourceFacilityRating { get; set; }
        public int SourceFacilityRatingNumber { get; set; }
        public string Distination { get; set; }
        public decimal DestinationFacilityRating { get; set; }
        public int DestinationFacilityRatingNumber { get; set; }
        public RoutePointStatus Status { get; set; }
        public ShippingRequestTripStatus TripStatus { get; set; }
        public string StatusTitle { get; set; }
        public ShippingRequestTripDriverActionStatusDto ActionStatus { get; set; } = ShippingRequestTripDriverActionStatusDto.None;

        public ShippingRequestTripDriverStatus DriverStatus { get; set; }
        //public string ChangeStatusButtonTitle { get; set; }
        public bool IsConfirmReceiver { get; set; }
        public long? WaybillNumber { get; set; }

        public string PlateNumber { get; set; }
        public string TruckType { get; set; }
        public int? CurrentTripId { get; set; }
        public bool HasAttachment { get; set; }
        public bool NeedsDeliveryNote { get; set; }

    }
}