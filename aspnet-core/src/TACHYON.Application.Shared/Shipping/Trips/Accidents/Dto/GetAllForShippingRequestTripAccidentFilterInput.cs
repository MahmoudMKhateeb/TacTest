using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Dto;

namespace TACHYON.Shipping.Trips.Accidents.Dto
{
    public class GetAllForShippingRequestTripAccidentFilterInput : PagedAndSortedInputDto
    {
        public bool? IsResolve { get; set; }
        public int TripId { get; set; }
        public long? PointId { get; set; }
    }
}