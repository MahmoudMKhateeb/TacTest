using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Dashboards.Shipper.Dto;

namespace TACHYON.Dashboards.Host.TMS_HostDto
{
    public class GetCostVsSellingTruckAggregationTripsOutput
    {
        public List<ChartCategoryPairedValuesDto> Cost { get; set; }
        public List<ChartCategoryPairedValuesDto> Selling { get; set; }
        public List<ChartCategoryPairedValuesDto> Profit { get; set; }
    }
}
