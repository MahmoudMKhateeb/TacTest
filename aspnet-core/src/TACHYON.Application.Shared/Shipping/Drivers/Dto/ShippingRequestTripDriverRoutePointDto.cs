using Abp.Application.Services.Dto;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Routs.RoutPoints;

namespace TACHYON.Shipping.Drivers.Dto
{
   public class ShippingRequestTripDriverRoutePointDto:EntityDto<long>
    {
        public string DisplayName { get; set; }
        public PickingType PickingType { get; set; }
        public string Facility { get; set; }

        public string Address { get; set; }
        public string Code { get; set; }

        public double lat { get; set; }
        public double lng { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsComplete { get; set; }

        public double? Rating { get; set; }

    }
}
