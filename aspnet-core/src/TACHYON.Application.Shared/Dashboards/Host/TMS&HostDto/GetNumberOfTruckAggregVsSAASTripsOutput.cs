using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Dashboards.Shipper.Dto;

namespace TACHYON.Dashboards.Host.TMS_HostDto
{
    public class GetNumberOfTruckAggregVsSAASTripsOutput
    {
        public List<ChartCategoryPairedValuesDto> TruckAggregationTrips { get; set; }
        public List<ChartCategoryPairedValuesDto> SAASTrips { get; set; }
    }
}
