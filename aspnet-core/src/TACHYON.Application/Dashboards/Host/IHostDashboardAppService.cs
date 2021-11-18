using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Dashboards.Host.Dto;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Dashboards.Host
{
    public interface IHostDashboardAppService : IApplicationService
    {
        Task<List<TruckTypeAvailableTrucksDto>> GetTrucksTypeCount();
        Task<List<ListPerMonthDto>> GetAccountsCountsPerMonth();
        Task<List<ListPerMonthDto>> GetNewTripsCountPerMonth();
        Task<long> GetOngoingTripsCount();
        Task<long> GetDeliveredTripsCount();
        Task<long> GetShippersCount();
        Task<long> GetCarriersCount();
        Task<long> GetDriversCount();
        Task<long> GetTrucksCount();
        Task<List<GoodTypeAvailableDto>> GetGoodTypeCountPerMonth();
        Task<List<RouteTypeAvailableDto>> GetRouteTypeCountPerMonth();
    }
}