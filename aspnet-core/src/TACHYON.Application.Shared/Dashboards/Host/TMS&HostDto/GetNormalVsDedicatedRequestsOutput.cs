using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Dashboards.Shipper.Dto;

namespace TACHYON.Dashboards.Host.TMS_HostDto
{
    public class GetNormalVsDedicatedRequestsOutput
    {
        public List<ChartCategoryPairedValuesDto> NormalTrips { get; set; }
        public List<ChartCategoryPairedValuesDto> DedicatedTrips { get; set; }

    }
}
