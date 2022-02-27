using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Dashboards.Carrier.Dto;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Dashboards.Carrier
{
    public interface ICarrierDashboardAppService : IApplicationService
    {
        Task<ActivityItemsDto> GetDriversActivity();
        Task<ActivityItemsDto> GetTrucksActivity();
    }
}