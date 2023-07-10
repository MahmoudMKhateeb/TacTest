using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Dashboards.Shipper.Dto;

namespace TACHYON.Dashboards.Host.TMS_HostDto
{
    public class GetRegisteredCompaniesNumberInRangeDto
    {
        public List<ChartCategoryPairedValuesDto> ShippersList { get; set; }
        public List<ChartCategoryPairedValuesDto> CarriersList { get; set; }
        public List<ChartCategoryPairedValuesDto> SaasList { get; set; }
    }
}
