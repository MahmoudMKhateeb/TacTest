﻿using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.ShippingRequestTripVases.Dtos;

namespace TACHYON.Shipping.Trips.Dto
{
    public class ShippingRequestsTripForViewDto
    {
        public DateTime StartTripDate { get; set; }
        public DateTime EndTripDate { get; set; }
        public DateTime? StartWorking { get; set; }
        public DateTime? EndWorking { get; set; }
        public string Status { get; set; }
        public string Driver { get; set; }
        public string Truck { get; set; }
        public string OriginFacility { get; set; }
        public string DestinationFacility { get; set; }

        public ICollection<RoutPointDto> RoutPoints { get; set; }
        public ICollection<ShippingRequestTripVasDto> ShippingRequestTripVases { get; set; }
    }
}