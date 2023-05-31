using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Timing;
using DevExpress.XtraRichEdit.Import.Html;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Dashboards.Host.Dto;
using TACHYON.Dashboards.Host.TMS_HostDto;
using TACHYON.Dashboards.Shipper.Dto;
using TACHYON.MultiTenancy;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Trucks;

namespace TACHYON.Dashboards.Host
{
    [AbpAuthorize(AppPermissions.Pages_HostDashboard, AppPermissions.App_TachyonDealer)]
    public class TMSAndHostDashboard: TACHYONAppServiceBase, ITMSAndHostDashboard
    {
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<Truck, long> _truckRepository;
        private readonly UserManager _userManager;


        public TMSAndHostDashboard(IRepository<Tenant> tenantRepository, IRepository<ShippingRequest, long> shippingRequestRepository, IRepository<ShippingRequestTrip> shippingRequestTripRepository, IRepository<Truck, long> truckRepository, UserManager userManager)
        {
            _tenantRepository = tenantRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _truckRepository = truckRepository;
            _userManager = userManager;
        }

        public async Task<GetRegisteredCompaniesNumberOutput> GetRegisteredCompaniesNumber()
        {
            await DisableTenancyFiltersIfTachyonDealer();
            var dto = new GetRegisteredCompaniesNumberOutput
            {
                ShippersNumber = await _tenantRepository.GetAll().AsNoTracking()
                .Where(r => r.Edition.DisplayName.Equals(TACHYONConsts.ShipperEdtionName))
                .CountAsync(),
                CarriersCount = await _tenantRepository.GetAll().AsNoTracking()
                .Where(r => r.Edition.DisplayName.Equals(TACHYONConsts.CarrierEdtionName))
                .CountAsync()
            };
            dto.TotalNumber = dto.ShippersNumber + dto.CarriersCount;
            return dto;
        }

        public async Task<List<ChartCategoryPairedValuesDto>> GetRegisteredCompaniesNumberInRange(DateRangeInput input)
        {
            await DisableTenancyFiltersIfTachyonDealer();

            var list = (await _tenantRepository.GetAll().AsNoTracking()
            .Where(r => r.CreationTime.Date > input.StartDate && r.CreationTime.Date < input.EndDate).Select(x=>x.CreationTime).ToListAsync())
            .Select(creationTime => new { y = creationTime.Year + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(creationTime.Month) })
            .GroupBy(x => x.y).Select(x => new { X = x.Key, Y= x.Count()});

            var dto = new List<ChartCategoryPairedValuesDto>();
             foreach(var monthWithYear in MonthsWithYearsInRange(input.StartDate, input.EndDate))
             {
                var item = list.FirstOrDefault(x => x.X == monthWithYear);
                if (item != null)
                {
                    dto.Add(new ChartCategoryPairedValuesDto { X = item.X, Y = item.Y });
                }
                else
                {
                    dto.Add(new ChartCategoryPairedValuesDto { X = monthWithYear, Y = 0 });
                }
             }
            return dto;
        }


        public async Task<List<RouteTypeAvailableDto>> GetRouteTypeCount()
        {
            await DisableTenancyFiltersIfTachyonDealer();

            return await _shippingRequestTripRepository.GetAll().AsNoTracking().Where(x => x.ShippingRequestFk.RouteTypeId != null || x.RouteType != null)
                .Select(x=> new {routeType = x.ShippingRequestFk.RouteTypeId != null ? x.ShippingRequestFk.RouteTypeId : x.RouteType})
                .GroupBy(x => x.routeType)
                .Select(group => new RouteTypeAvailableDto
                {
                    RouteType = group.Key.GetEnumDescription(),
                    AvailableRouteTypesCount = group.Count()
                }).ToListAsync();
        }


        public async Task<List<ChartCategoryPairedValuesDto>> GetNumberOfTrips(DateRangeInput input)
        {
            await DisableTenancyFiltersIfTachyonDealer();
            var list = (await  _shippingRequestTripRepository.GetAll()
                .Where(x => (x.ShippingRequestFk != null && x.ShippingRequestFk.TenantId != x.ShippingRequestFk.CarrierTenantId) ||
                            (x.ShippingRequestFk == null && x.ShipperTenantId != x.CarrierTenantId) &&
                            x.CreationTime.Date > input.StartDate && x.CreationTime < input.EndDate)
                .Select(x=>x.CreationTime)
                .ToListAsync())
                            .Select(creationTime => new { y = creationTime.Year + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(creationTime.Month) })
                            .GroupBy(x => x.y)
                            .Select(x => new { X = x.Key, Y = x.Count() });

            var dto = new List<ChartCategoryPairedValuesDto>();
            foreach (var monthWithYear in MonthsWithYearsInRange(input.StartDate, input.EndDate))
            {
                var item = list.FirstOrDefault(x => x.X == monthWithYear);
                if (item != null)
                {
                    dto.Add(new ChartCategoryPairedValuesDto { X = item.X, Y = item.Y });
                }
                else
                {
                    dto.Add(new ChartCategoryPairedValuesDto { X = monthWithYear, Y = 0 });
                }
            }
            return dto;
        }

        public async Task<long> GetDeliveredTripsInCurrentMonth()
        {
            return await _shippingRequestTripRepository.GetAll().Where(x => (x.ShippingRequestFk != null && x.ShippingRequestFk.TenantId != x.ShippingRequestFk.CarrierTenantId) ||
                           (x.ShippingRequestFk == null && x.ShipperTenantId != x.CarrierTenantId) &&
                           x.CreationTime.Month == Clock.Now.Month && (
                           x.Status == ShippingRequestTripStatus.Delivered || x.Status == ShippingRequestTripStatus.DeliveredAndNeedsConfirmation))
                           .CountAsync();
        }

        public async Task<long> GetInTransitTripsInCurrentMonth()
        {
            return await _shippingRequestTripRepository.GetAll().Where(x => (x.ShippingRequestFk != null && x.ShippingRequestFk.TenantId != x.ShippingRequestFk.CarrierTenantId) ||
                            (x.ShippingRequestFk == null && x.ShipperTenantId != x.CarrierTenantId) &&
                            x.CreationTime.Month == Clock.Now.Month && 
                            x.Status == ShippingRequestTripStatus.InTransit)
                            .CountAsync();
        }

        public async Task<DriversAndTrucksDto> GetDriversAndTrucksCount(DateRangeInput input)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();

            var trucksList = (await _truckRepository.GetAll().Where(x=>x.CreationTime.Date > input.StartDate && x.CreationTime.Date < input.EndDate)
                .Select(x=>x.CreationTime).ToListAsync()).Select(creationTime => new { y = creationTime.Year + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(creationTime.Month) })
                            .GroupBy(x => x.y)
                            .Select(x => new { X = x.Key, Y = x.Count() });
            var driversList = (await _userManager.Users.Where(x=>x.IsDriver && x.CreationTime.Date > input.StartDate && x.CreationTime.Date < input.EndDate)
                .Select(x => x.CreationTime).ToListAsync()).Select(creationTime => new { y = creationTime.Year + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(creationTime.Month) })
                            .GroupBy(x => x.y)
                            .Select(x => new { X = x.Key, Y = x.Count() });


            var dto = new DriversAndTrucksDto
            {
                Drivers = new List<ChartCategoryPairedValuesDto>(),
                Trucks = new List<ChartCategoryPairedValuesDto>()
            };

            foreach (var monthWithYear in MonthsWithYearsInRange(input.StartDate, input.EndDate))
            {
                var truck = trucksList.FirstOrDefault(x => x.X == monthWithYear);
                if (truck != null)
                {
                    dto.Trucks.Add(new ChartCategoryPairedValuesDto { X = truck.X, Y = truck.Y });
                }
                else
                {
                    dto.Trucks.Add(new ChartCategoryPairedValuesDto { X = monthWithYear, Y = 0 });
                }

                var driver = driversList.FirstOrDefault(x => x.X == monthWithYear);
                if (driver != null)
                {
                    dto.Drivers.Add(new ChartCategoryPairedValuesDto { X = driver.X, Y = driver.Y });
                }
                else
                {
                    dto.Drivers.Add(new ChartCategoryPairedValuesDto { X = monthWithYear, Y = 0 });
                }
            }
            return dto;
        }


        public async Task<List<GetTopOWorstRatedTenantsOutput>> GetTopOWorstRatedTenants(GetTopOWorstRatedTenantsInput input)
        {
            await DisableTenancyFiltersIfTachyonDealer();
            var tenants = _tenantRepository.GetAll().AsNoTracking();
            var tenantList = default(List<Tenant>);

            switch (input.EditionType)
            {
                case 1:
                    tenantList = await tenants.Where(r => r.Edition.DisplayName.Contains(TACHYONConsts.ShipperEdtionName)).ToListAsync();
                    break;
                case 2:
                    tenantList = await tenants.Where(r => r.Edition.DisplayName.Contains(TACHYONConsts.CarrierEdtionName)).ToListAsync();
                    break;
                case 3:
                    tenantList = await tenants.Where(r => r.Edition.DisplayName.Contains(TACHYONConsts.BrokerEditionName) ||
                                     r.Edition.DisplayName.Contains(TACHYONConsts.CarrierSaasEditionName)).ToListAsync();
                    break;
            }

            switch (input.RateType)
            {
                case 1:
                    tenantList = tenantList.OrderByDescending(x => x.Rate).ToList();
                    break;
                case 2:
                    tenantList = tenantList.OrderBy(x => x.Rate).ToList();
                    break;
            }

            var tenantsIdsList = tenantList.Select(x => x.Id).ToList();

            var trips = await _shippingRequestTripRepository.GetAll().AsNoTracking()
                .Where(r => (r.Status == ShippingRequestTripStatus.Delivered || r.Status == ShippingRequestTripStatus.DeliveredAndNeedsConfirmation)
                && tenantsIdsList.Contains(r.ShippingRequestFk.TenantId) || (r.ShipperTenantId!= null && tenantsIdsList.Contains(r.ShipperTenantId.Value)) ||
                tenantsIdsList.Contains(r.ShippingRequestFk.CarrierTenantId.Value) || (r.CarrierTenantId != null && tenantsIdsList.Contains(r.CarrierTenantId.Value)))
                .Select(x=> new { x.CarrierTenantId , x.ShipperTenantId, requestShippers = x.ShippingRequestFk.TenantId, requestCarriers = x.ShippingRequestFk.CarrierTenantId }).ToListAsync();

            return tenantList.Select(tenant => new GetTopOWorstRatedTenantsOutput()
            {
                Id = tenant.Id,
                Name = tenant.companyName,
                Rating = tenant.Rate,
                NumberOfTrips = trips.Where(x=>x.CarrierTenantId == tenant.Id || x.ShipperTenantId == tenant.Id ||
                x.requestShippers == tenant.Id || x.requestCarriers == tenant.Id).Count()
            }).Take(5).ToList();
        }



        #region Helper
        private IEnumerable<string> MonthsWithYearsInRange(DateTime start, DateTime end)
        {
            for (DateTime date = start; date <= end; date = date.AddMonths(1))
            {
                yield return date.Year + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month);
            }
        }

        #endregion
    }
}
