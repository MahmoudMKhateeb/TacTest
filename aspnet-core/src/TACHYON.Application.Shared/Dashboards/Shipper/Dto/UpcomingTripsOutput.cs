using System;
using System.Collections.Generic;

namespace TACHYON.Dashboards.Shipper.Dto
{
    public class UpcomingTripsOutput
    {
        public DateTime Date { get; set; }

        public List<UpcomingTripItemDto> Trips { get; set; }
    }
}