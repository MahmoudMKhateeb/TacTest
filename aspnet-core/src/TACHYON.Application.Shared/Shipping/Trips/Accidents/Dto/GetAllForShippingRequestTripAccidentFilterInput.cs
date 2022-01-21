using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Trips.Accidents.Dto
{
    public class GetAllForShippingRequestTripAccidentFilterInput
    {
        public bool? IsResolve { get; set; }
        public int TripId { get; set; }
        public long? PointId { get; set; }

        public string Sorting { get; set; }
    }
}