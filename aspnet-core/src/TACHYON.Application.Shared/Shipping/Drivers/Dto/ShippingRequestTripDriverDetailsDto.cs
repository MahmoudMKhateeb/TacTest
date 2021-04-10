using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Abp.Application.Services.Dto;
using TACHYON.Shipping.Trips;

namespace TACHYON.Shipping.Drivers.Dto
{
    public class ShippingRequestTripDriverDetailsDto: EntityDto<long>
    {
        public ICollection<ShippingRequestTripDriverRoutePointDto> RoutePoints { get; set; }
        public string StartTripDate { get; set; }
        public string EndTripDate { get; set; }
        public string TravelTime { get; set; }
        public string PackingType { get; set; }

        public double TotalWeight { get; set; }

        public string UnitOfMeasure { get; set; } = "Kg";
        public string Note { get; set; }

        public string Source { get; set; }
        public string Distination { get; set; }

        public ShippingRequestTripStatus Status { get; set; }
        public string StatusTitle { get; set; }
        public ShippingRequestTripDriverActionStatusDto ActionStatus { get; set; } = ShippingRequestTripDriverActionStatusDto.None;

        public ShippingRequestTripDriverStatus DriverStatus { get; set; }
        public string ChangeStatusButtonTitle { get; set; }
        public bool IsConfirmReceiver { get; set; }

    }
}
