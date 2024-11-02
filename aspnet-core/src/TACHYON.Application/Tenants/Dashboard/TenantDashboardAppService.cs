using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Timing;
using Microsoft.EntityFrameworkCore;
using RestSharp.Extensions;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Dashboards.Shared.Dto;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Tenants.Dashboard.Dto;

namespace TACHYON.Tenants.Dashboard
{
    [DisableAuditing]
    [AbpAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
    public class TenantDashboardAppService : TACHYONAppServiceBase, ITenantDashboardAppService
    {
       private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;

        public TenantDashboardAppService(IRepository<ShippingRequestTrip> shippingRequestTripRepository)
        {
            _shippingRequestTripRepository = shippingRequestTripRepository;
        }

        public GetMemberActivityOutput GetMemberActivity()
        {
            return new GetMemberActivityOutput
            (
                DashboardRandomDataGenerator.GenerateMemberActivities()
            );
        }

        public GetDashboardDataOutput GetDashboardData(GetDashboardDataInput input)
        {
            var output = new GetDashboardDataOutput
            {
                TotalProfit = DashboardRandomDataGenerator.GetRandomInt(5000, 9000),
                NewFeedbacks = DashboardRandomDataGenerator.GetRandomInt(1000, 5000),
                NewOrders = DashboardRandomDataGenerator.GetRandomInt(100, 900),
                NewUsers = DashboardRandomDataGenerator.GetRandomInt(50, 500),
                SalesSummary = DashboardRandomDataGenerator.GenerateSalesSummaryData(input.SalesSummaryDatePeriod),
                Expenses = DashboardRandomDataGenerator.GetRandomInt(5000, 10000),
                Growth = DashboardRandomDataGenerator.GetRandomInt(5000, 10000),
                Revenue = DashboardRandomDataGenerator.GetRandomInt(1000, 9000),
                TotalSales = DashboardRandomDataGenerator.GetRandomInt(10000, 90000),
                TransactionPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                NewVisitPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                BouncePercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                DailySales = DashboardRandomDataGenerator.GetRandomArray(30, 10, 50),
                ProfitShares = DashboardRandomDataGenerator.GetRandomPercentageArray(3)
            };

            return output;
        }

        public GetTopStatsOutput GetTopStats()
        {
            return new GetTopStatsOutput
            {
                TotalProfit = DashboardRandomDataGenerator.GetRandomInt(5000, 9000),
                NewFeedbacks = DashboardRandomDataGenerator.GetRandomInt(1000, 5000),
                NewOrders = DashboardRandomDataGenerator.GetRandomInt(100, 900),
                NewUsers = DashboardRandomDataGenerator.GetRandomInt(50, 500)
            };
        }

        public GetProfitShareOutput GetProfitShare()
        {
            return new GetProfitShareOutput { ProfitShares = DashboardRandomDataGenerator.GetRandomPercentageArray(3) };
        }

        public GetDailySalesOutput GetDailySales()
        {
            return new GetDailySalesOutput { DailySales = DashboardRandomDataGenerator.GetRandomArray(30, 10, 50) };
        }

        public GetSalesSummaryOutput GetSalesSummary(GetSalesSummaryInput input)
        {
            var salesSummary = DashboardRandomDataGenerator.GenerateSalesSummaryData(input.SalesSummaryDatePeriod);
            return new GetSalesSummaryOutput(salesSummary)
            {
                Expenses = DashboardRandomDataGenerator.GetRandomInt(0, 3000),
                Growth = DashboardRandomDataGenerator.GetRandomInt(0, 3000),
                Revenue = DashboardRandomDataGenerator.GetRandomInt(0, 3000),
                TotalSales = DashboardRandomDataGenerator.GetRandomInt(0, 3000)
            };
        }

        public GetRegionalStatsOutput GetRegionalStats()
        {
            return new GetRegionalStatsOutput(
                DashboardRandomDataGenerator.GenerateRegionalStat()
            );
        }

        public GetGeneralStatsOutput GetGeneralStats()
        {
            return new GetGeneralStatsOutput
            {
                TransactionPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                NewVisitPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                BouncePercent = DashboardRandomDataGenerator.GetRandomInt(10, 100)
            };
        }
    
         public async Task<ContainerReturnTrackerWidgetDataDto> GetContainerReturnTrackerWidgetData(int UpcomingUnreturnedContainersFactor)
        {
            DisableTenancyFilters();

            var list = await _shippingRequestTripRepository
                .GetAll()
                .AsNoTracking()
                .Where(x => x.ShippingRequestFk.TenantId == AbpSession.TenantId || x.ShipperTenantId == AbpSession.TenantId || x.CarrierTenantId == AbpSession.TenantId)
                .Where(x => x.IsContainerReturned != true)
                .Where(x=> x.ContainerNumber != null)
                .Select(x => new
                {
                    x.ContainerReturnDate,
                    x.Id
                })
                .ToListAsync();

                return new ContainerReturnTrackerWidgetDataDto()
                {
                    TotalUnreturnedContainers = list.Count(),
                    DelayedUnreturnedContainers = list.Count(x=> x.ContainerReturnDate >= Clock.Now),
                    UpcomingUnreturnedContainers = list.Count(x=> x.ContainerReturnDate >= Clock.Now.AddDays(-1 * UpcomingUnreturnedContainersFactor)),

                };





        }


    
    }
}