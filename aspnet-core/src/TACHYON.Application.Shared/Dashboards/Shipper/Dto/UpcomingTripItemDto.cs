using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace TACHYON.Dashboards.Shipper.Dto
{
    public class UpcomingTripItemDto : EntityDto
    {
        public long? WaybillNumber { get; set; }

        public string Origin { get; set; }

        public List<string> Destinations { get; set; }

        public string TripType { get; set; }

        public bool IsDirectTrip { get; set; }
    }
}