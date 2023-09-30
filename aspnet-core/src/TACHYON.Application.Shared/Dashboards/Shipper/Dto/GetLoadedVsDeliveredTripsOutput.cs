using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Dashboards.Shipper.Dto
{
    public class GetLoadedVsDeliveredTripsOutput
    {
        public List<ChartCategoryPairedValuesDto> LoadedTrips { get; set; }
        public List<ChartCategoryPairedValuesDto> DeliveredTrips { get; set; }
    }
}
