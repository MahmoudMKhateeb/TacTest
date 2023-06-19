using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Dashboards.Host.Dto;
using TACHYON.Dashboards.Host.TMS_HostDto;
using TACHYON.Dashboards.Shipper.Dto;

namespace TACHYON.Dashboards.Host
{
    public interface ITMSAndHostDashboard
    {
        Task<GetRegisteredCompaniesNumberOutput> GetRegisteredCompaniesNumber();
        Task<List<ChartCategoryPairedValuesDto>> GetRegisteredCompaniesNumberInRange(DateRangeInput input);
        Task<List<RouteTypeAvailableDto>> GetRouteTypeCount();

    }
}
