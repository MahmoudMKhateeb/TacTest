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
        Task<List<ListPerMonthDto>> GetAccountsStatistics(GetDataByDateFilterInput input);
        Task<List<ListPerMonthDto>> GetNewTripsStatistics(GetDataByDateFilterInput input);
        Task<long> GetOngoingTripsCount();
        Task<long> GetDeliveredTripsCount();
        Task<long> GetShippersCount();
        Task<long> GetCarriersCount();
        Task<long> GetDriversCount();
        Task<long> GetTrucksCount();
        Task<List<GoodTypeAvailableDto>> GetGoodTypeCountPerMonth();
        Task<List<RouteTypeAvailableDto>> GetRouteTypeCountPerMonth();
        Task<List<ListUsersHaveMostRequests>> GetShippersHaveMostRequests();
        Task<List<ListUsersHaveMostRequests>> GetCarriersHaveMostRequests();
        Task<List<ListUsersHaveMostRequests>> GetTopRatedShippers();
        Task<List<ListUsersHaveMostRequests>> GetTopRatedCarriers();
        Task<List<ListUsersHaveMostRequests>> GetWorstRatedShippers();
        Task<List<ListRequestsByCityDto>> GetNumberOfRequestsForEachCity();
        Task<List<ListRequestsUnPricedMarketPlace>> GetUnpricedRequestsInMarketplace(GetDataByDateFilterInput input);
        Task<List<ListUsersHaveMostRequests>> GetWorstRatedCarriers();
        Task<long> GetRequestBeingPricedBeforeBidEndDateCount();
        Task<long> GetRequestsPricingAcceptedCount();
        Task<long> GetInvoicesPaidBeforeDueDatePercentageCount();

    }
}