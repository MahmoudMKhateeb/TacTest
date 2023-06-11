using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Dashboards.Shipper.Dto;

namespace TACHYON.Dashboards.Host.TMS_HostDto
{
    public class DriversAndTrucksDto
    {
        public List<ChartCategoryPairedValuesDto> Drivers { get; set; }
        public List<ChartCategoryPairedValuesDto> Trucks { get; set; }
    }
}
