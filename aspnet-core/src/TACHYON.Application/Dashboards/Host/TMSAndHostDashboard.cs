using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Timing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Dashboards.Carrier;
using TACHYON.Dashboards.Host.Dto;
using TACHYON.Dashboards.Host.TMS_HostDto;
using TACHYON.Dashboards.Shipper.Dto;
using TACHYON.Invoices;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.MultiTenancy;
using TACHYON.Routs.RoutPoints;
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
        private readonly IRepository<Invoice, long> _invoiceRepository;
        private readonly IRepository<SubmitInvoice, long> _submitInvoiceRepository;
        private readonly IRepository<RoutPoint, long> _routePointRepository;



        public TMSAndHostDashboard(IRepository<Tenant> tenantRepository, IRepository<ShippingRequest, long> shippingRequestRepository, IRepository<ShippingRequestTrip> shippingRequestTripRepository, IRepository<Truck, long> truckRepository, UserManager userManager, IRepository<Invoice, long> invoiceRepository, IRepository<SubmitInvoice, long> submitInvoiceRepository, IRepository<RoutPoint, long> routePointRepository)
        {
            _tenantRepository = tenantRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _truckRepository = truckRepository;
            _userManager = userManager;
            _invoiceRepository = invoiceRepository;
            _submitInvoiceRepository = submitInvoiceRepository;
            _routePointRepository = routePointRepository;
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
                .CountAsync(),
                SAASCount = await _tenantRepository.GetAll().AsNoTracking()
                .Where(r => r.Edition.DisplayName.Equals(TACHYONConsts.BrokerEditionName) || r.Edition.DisplayName.Equals(TACHYONConsts.CarrierSaasEditionName))
                .CountAsync()
            };
            dto.TotalNumber = dto.ShippersNumber + dto.CarriersCount + dto.SAASCount;
            return dto;
        }

        public async Task<GetRegisteredCompaniesNumberInRangeDto> GetRegisteredCompaniesNumberInRange(DateRangeInput input)
        {
            await DisableTenancyFiltersIfTachyonDealer();

            var list = _tenantRepository.GetAll().AsNoTracking()
            .Where(r => r.CreationTime.Date > input.StartDate && r.CreationTime.Date < input.EndDate && !r.Name.Equals("Default"));


            var shippers = (await list.Where(x=>x.Edition.DisplayName == TACHYONConsts.ShipperEdtionName).Select(x => x.CreationTime)
            .ToListAsync())
            .Select(creationTime => new { y = creationTime.Year + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(creationTime.Month) })
            .GroupBy(x => x.y).Select(x => new { X = x.Key, Y= x.Count()}).ToList();

            var carriers = (await list.Where(x => x.Edition.DisplayName == TACHYONConsts.CarrierEdtionName).Select(x => x.CreationTime)
            .ToListAsync())
            .Select(creationTime => new { y = creationTime.Year + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(creationTime.Month) })
            .GroupBy(x => x.y).Select(x => new { X = x.Key, Y = x.Count() }).ToList();

            var saas = (await list.Where(x => x.Edition.DisplayName.Equals(TACHYONConsts.BrokerEditionName) || x.Edition.DisplayName.Equals(TACHYONConsts.CarrierSaasEditionName)).Select(x => x.CreationTime)
            .ToListAsync())
            .Select(creationTime => new { y = creationTime.Year + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(creationTime.Month) })
            .GroupBy(x => x.y).Select(x => new { X = x.Key, Y = x.Count() }).ToList();

            var dto = new GetRegisteredCompaniesNumberInRangeDto
            {
                CarriersList = new List<ChartCategoryPairedValuesDto>(),
                ShippersList = new List<ChartCategoryPairedValuesDto>(),
                SaasList = new List<ChartCategoryPairedValuesDto>()
            };

            foreach (var monthWithYear in MonthsWithYearsInRange(input.StartDate, input.EndDate))
             {
                var shipper = shippers.FirstOrDefault(x => x.X == monthWithYear);
                if (shipper != null)
                {
                    dto.ShippersList.Add(new ChartCategoryPairedValuesDto { X = shipper.X, Y = shipper.Y });
                }
                else
                {
                    dto.ShippersList.Add(new ChartCategoryPairedValuesDto { X = monthWithYear, Y = 0 });
                }

                var carrier = carriers.FirstOrDefault(x => x.X == monthWithYear);
                if (carrier != null)
                {
                    dto.CarriersList.Add(new ChartCategoryPairedValuesDto { X = carrier.X, Y = carrier.Y });
                }
                else
                {
                    dto.CarriersList.Add(new ChartCategoryPairedValuesDto { X = monthWithYear, Y = 0 });
                }


                var saasItem = saas.FirstOrDefault(x => x.X == monthWithYear);
                if (saasItem != null)
                {
                    dto.SaasList.Add(new ChartCategoryPairedValuesDto { X = saasItem.X, Y = saasItem.Y });
                }
                else
                {
                    dto.SaasList.Add(new ChartCategoryPairedValuesDto { X = monthWithYear, Y = 0 });
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
            DisableTenancyFilters();
            return await _shippingRequestTripRepository.GetAll().Where(x => ((x.ShippingRequestFk != null && x.ShippingRequestFk.TenantId != x.ShippingRequestFk.CarrierTenantId) ||
                           (x.ShippingRequestFk == null && x.ShipperTenantId != x.CarrierTenantId)) &&
                           x.CreationTime.Month == Clock.Now.Month && (
                           x.Status == ShippingRequestTripStatus.Delivered || x.Status == ShippingRequestTripStatus.DeliveredAndNeedsConfirmation))
                           .CountAsync();
        }

        public async Task<long> GetInTransitTripsInCurrentMonth()
        {
            DisableTenancyFilters();
            return await _shippingRequestTripRepository.GetAll().Where(x => ((x.ShippingRequestFk != null && x.ShippingRequestFk.TenantId != x.ShippingRequestFk.CarrierTenantId) ||
                            (x.ShippingRequestFk == null && x.ShipperTenantId != x.CarrierTenantId)) &&
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
            var tenants = _tenantRepository.GetAll().Where(r => !r.Name.Equals("Default") && r.Rate > 0).AsNoTracking();
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

        public async Task<List<GetTruckTypeUsageOutput>> GetTruckTypeUsage(int transportTypeId)
        {
            DisableTenancyFilters();
                var trips =await _shippingRequestTripRepository.GetAll()
            .Where(x => (x.Status == ShippingRequestTripStatus.Delivered || 
            x.Status == ShippingRequestTripStatus.DeliveredAndNeedsConfirmation) &&
            x.AssignedTruckFk.TransportTypeId == transportTypeId)
            .Select(x => x.AssignedTruckFk)
            .Select(x => new
            {
                TransportType = x.TransportTypeFk.DisplayName,
                truckType = x.TrucksTypeFk.DisplayName +"-"+ x.CapacityFk.DisplayName
            }).Where(x=> !string.IsNullOrEmpty(x.truckType))
            .GroupBy(x => x.truckType)
            .Select(g => new
            {
                name = g.Key,
                trips = g.Count()
            })
            .OrderByDescending(g => g.trips)
            .Take(5).ToListAsync();

            return trips.Select(x => new GetTruckTypeUsageOutput
            {
                Name = x.name,
                NumberOfTrips = x.trips
            }).ToList();
        }

        public async Task<List<GetTruckTypeUsageOutput>> GetGoodsUsage(int goodsCategoryId, DateRangeInput dateRangeInput)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();

            var goodsDetailsList =await _shippingRequestTripRepository.GetAll()
                .Where(x => (x.Status == ShippingRequestTripStatus.Delivered ||
                x.Status == ShippingRequestTripStatus.DeliveredAndNeedsConfirmation) &&
                x.GoodCategoryId == goodsCategoryId || x.ShippingRequestFk.GoodCategoryId == goodsCategoryId &&
                x.CreationTime.Date > dateRangeInput.StartDate && x.CreationTime.Date < dateRangeInput.EndDate)
                .SelectMany(x => x.RoutPoints.Where(x=>x.PickingType == Routs.RoutPoints.PickingType.Dropoff)
                .SelectMany(y => y.GoodsDetails))
                .Select(x => new
                {
                    goodsSubCatName = x.GoodCategoryFk.Key,
                })
            .GroupBy(x => x.goodsSubCatName)
            .Select(g => new
            {
                name = g.Key,
                trips = g.Count()
            })
            .OrderByDescending(g => g.trips)
            .Take(5).ToListAsync();

                return goodsDetailsList.Select(x => new GetTruckTypeUsageOutput
                {
                    Name = x.name,
                    NumberOfTrips = x.trips
                }).ToList();
        }


        public async Task<GetTopTenantsCreatedTripsOutput> GetTopTenantsCreatedTrips(DateRangeInput input)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();

            var shippers = await _shippingRequestTripRepository.GetAll()
                .Where(x=>x.CreationTime.Date > input.StartDate && x.CreationTime.Date < input.EndDate)
                .Select(x => new
                {
                    tenant = x.ShippingRequestFk != null ? x.ShippingRequestFk.Tenant.companyName
                    : x.ShipperTenantFk.companyName,
                    rate = x.ShippingRequestFk != null ?  x.ShippingRequestFk.Tenant.Rate
                    :  x.ShipperTenantFk.Rate,
                }).GroupBy(x => new {  x.tenant, x.rate })
                .Select(x => new GetTenantsCountWithRateOutput
                {
                    Name = x.Key.tenant,
                    Rate = x.Key.rate,
                    NumberOfTrips = x.Count()
                })
                .OrderByDescending(x => x.NumberOfTrips)
                .Take(3).ToListAsync();

            var carriers = await _shippingRequestTripRepository.GetAll()
                .Where(x => x.CreationTime.Date > input.StartDate && x.CreationTime.Date < input.EndDate)
                .Select(x => new
                {
                    tenant = x.ShippingRequestFk != null && x.ShippingRequestFk.CarrierTenantId != null
                    ? x.ShippingRequestFk.CarrierTenantFk.companyName
                    : x.CarrierTenantFk.companyName,
                    rate = x.ShippingRequestFk != null ? x.ShippingRequestFk.Tenant.Rate
                    : x.ShipperTenantFk.Rate,
                }).GroupBy(x =>new { x.tenant , x.rate})
                .Select(x => new GetTenantsCountWithRateOutput
                {
                    Name = x.Key.tenant,
                    Rate = x.Key.rate,
                    NumberOfTrips = x.Count()
                })
                .OrderByDescending(x => x.NumberOfTrips)
                .Take(3).ToListAsync();

            return new GetTopTenantsCreatedTripsOutput { Carriers = carriers, Shippers = shippers };
        }


        public async Task<GetNormalVsDedicatedRequestsOutput> GetNormalVsDedicatedRequests(DateRangeInput input)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var total = _shippingRequestRepository.GetAll()
                .Where(x => x.CreationTime.Date > input.StartDate && x.CreationTime.Date < input.EndDate);

            var normalTrips = (await total.Where(x => x.ShippingRequestFlag == ShippingRequestFlag.Normal)
                .Select(x => x.CreationTime.Date).ToListAsync())
                .Select(creationTime =>
                new { y = creationTime.Year + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(creationTime.Month) })
                .GroupBy(x => x.y)
                .Select(x => new { X = x.Key, Y = x.Count() })
                .ToList();

            var dedicatedTrips = (await total.Where(x => x.ShippingRequestFlag == ShippingRequestFlag.Dedicated)
                .Select(x => x.CreationTime.Date).ToListAsync())
                .Select(creationTime =>
                new { y = creationTime.Year + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(creationTime.Month) })
                .GroupBy(x => x.y)
                .Select(x => new { X = x.Key, Y = x.Count() })
                .ToList();


            var dto = new GetNormalVsDedicatedRequestsOutput
            {
                DedicatedTrips = new List<ChartCategoryPairedValuesDto>(),
                NormalTrips = new List<ChartCategoryPairedValuesDto>()
            };
            foreach (var monthWithYear in MonthsWithYearsInRange(input.StartDate, input.EndDate))
            {
                var normal = normalTrips.FirstOrDefault(x => x.X == monthWithYear);
                if (normal != null)
                {
                    dto.NormalTrips.Add(new ChartCategoryPairedValuesDto { X = normal.X, Y = normal.Y });
                }
                else
                {
                    dto.NormalTrips.Add(new ChartCategoryPairedValuesDto { X = monthWithYear, Y = 0 });
                }

                var dedicated = dedicatedTrips.FirstOrDefault(x => x.X == monthWithYear);
                if (dedicated != null)
                {
                    dto.DedicatedTrips.Add(new ChartCategoryPairedValuesDto { X = dedicated.X, Y = dedicated.Y });
                }
                else
                {
                    dto.DedicatedTrips.Add(new ChartCategoryPairedValuesDto { X = monthWithYear, Y = 0 });
                }
            }

            return dto;

        }


        public async Task<long> GetSaasTripsNumber()
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            return await _shippingRequestTripRepository.GetAll()
                .Where(x=>x.CreationTime.Month == Clock.Now.Month)
                .Where(x => x.ShippingRequestFk.TenantId == x.ShippingRequestFk.CarrierTenantId ||
            (x.ShippingRequestFk == null && x.ShipperTenantId == x.CarrierTenantId)).CountAsync();
        }

        public async Task<long> GetTruckAggregationTripsNumber()
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            return await _shippingRequestTripRepository.GetAll()
                .Where(x => x.CreationTime.Month == Clock.Now.Month)
                .Where(x => x.ShippingRequestFk.TenantId != x.ShippingRequestFk.CarrierTenantId ||
            (x.ShipperTenantId != x.CarrierTenantId)).CountAsync();
        }

        public async Task<GetInvoicesPaidVsUnpaidOutput> GetInvoicesPaidVsUnpaid(DateRangeInput input)
        {
            DisableTenancyFilters();

            var query = _invoiceRepository
                .GetAll().Where(x => x.CreationTime.Date > input.StartDate && x.CreationTime.Date < input.EndDate)
                .AsNoTracking();

            var paid = (await query
                    .Where(x => x.IsPaid).Select(x=>x.CreationTime.Date)
                    .ToListAsync())
                    .Select(creationTime =>
                new { y = creationTime.Year + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(creationTime.Month) })
                .GroupBy(x => x.y)
                .Select(x => new { X = x.Key, Y = x.Count() })
                .ToList();

            var unPaid = (await query
                .Where(x => !x.IsPaid).Select(x => x.CreationTime.Date)
                    .ToListAsync())
                    .Select(creationTime =>
                new { y = creationTime.Year + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(creationTime.Month) })
                .GroupBy(x => x.y)
                .Select(x => new { X = x.Key, Y = x.Count() })
                .ToList();

            var dto = new GetInvoicesPaidVsUnpaidOutput
            {
                PaidInvoices = new List<ChartCategoryPairedValuesDto>(),
                UnPaidInvoices = new List<ChartCategoryPairedValuesDto>()
            };
            foreach (var monthWithYear in MonthsWithYearsInRange(input.StartDate, input.EndDate))
            {
                var paidItem = paid.FirstOrDefault(x => x.X == monthWithYear);
                if (paidItem != null)
                {
                    dto.PaidInvoices.Add(new ChartCategoryPairedValuesDto { X = paidItem.X, Y = paidItem.Y });
                }
                else
                {
                    dto.PaidInvoices.Add(new ChartCategoryPairedValuesDto { X = monthWithYear, Y = 0 });
                }


                var unPaidItem = unPaid.FirstOrDefault(x => x.X == monthWithYear);
                if (paidItem != null)
                {
                    dto.UnPaidInvoices.Add(new ChartCategoryPairedValuesDto { X = unPaidItem.X, Y = unPaidItem.Y });
                }
                else
                {
                    dto.UnPaidInvoices.Add(new ChartCategoryPairedValuesDto { X = monthWithYear, Y = 0 });
                }
            }


            return dto;
        }


        public async Task<GetCarrierInvoicesDetailsOutput> GetClaimedVsPaidDetails(DateRangeInput input)
        {
            DisableTenancyFilters();


            var query = _submitInvoiceRepository
                .GetAll()
                .Where(x => x.CreationTime.Date > input.StartDate && x.CreationTime.Date < input.EndDate)
                .AsNoTracking();

            var paid = (await query
                    .Where(x => x.Status == SubmitInvoiceStatus.Paid)
                    .Select(x => x.CreationTime.Date)
                    .ToListAsync())
                    .Select(creationTime =>
                new { y = creationTime.Year + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(creationTime.Month) })
                .GroupBy(x => x.y)
                .Select(x => new { X = x.Key, Y = x.Count() })
                .ToList();


            var claimed = (await query
               .Where(x => x.Status == SubmitInvoiceStatus.Claim)
               .Select(x => x.CreationTime.Date)
                    .ToListAsync())
                    .Select(creationTime =>
                new { y = creationTime.Year + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(creationTime.Month) })
                .GroupBy(x => x.y)
                .Select(x => new { X = x.Key, Y = x.Count() })
                .ToList();

            var dto = new GetCarrierInvoicesDetailsOutput
            {
                Claimed = new List<ChartCategoryPairedValuesDto>(),
                PaidInvoices = new List<ChartCategoryPairedValuesDto>()
            };
            foreach (var monthWithYear in MonthsWithYearsInRange(input.StartDate, input.EndDate))
            {
                var paidItem = paid.FirstOrDefault(x => x.X == monthWithYear);
                if (paidItem != null)
                {
                    dto.PaidInvoices.Add(new ChartCategoryPairedValuesDto { X = paidItem.X, Y = paidItem.Y });
                }
                else
                {
                    dto.PaidInvoices.Add(new ChartCategoryPairedValuesDto { X = monthWithYear, Y = 0 });
                }


                var claimedItem = claimed.FirstOrDefault(x => x.X == monthWithYear);
                if (claimedItem != null)
                {
                    dto.Claimed.Add(new ChartCategoryPairedValuesDto { X = claimedItem.X, Y = claimedItem.Y });
                }
                else
                {
                    dto.Claimed.Add(new ChartCategoryPairedValuesDto { X = monthWithYear, Y = 0 });
                }

            }

            return dto;

        }


        public async Task<List<GetRequestsHeapMapOutput>> GetRequestsHeapMap(DateRangeInput input)
        {
            DisableTenancyFilters();
            var origins = await _shippingRequestRepository.GetAll()
                .Where(x => x.CreationTime.Date > input.StartDate && x.CreationTime.Date < input.EndDate && x.OriginCityId != null)
                .Select(x => new { origin = x.OriginCityFk.DisplayName, x.OriginCityId})
                .GroupBy(x => new { x.origin , x.OriginCityId})
                .OrderByDescending(x => x.Count())
                .Select(x => new GetRequestsHeapMapOutput
                {
                    CityType = "origin",
                    NumberOfRequests = x.Count(),
                    CityName = x.Key.origin,
                    CityId = x.Key.OriginCityId.Value
                }).Take(5).ToListAsync();


            var destinations =await _shippingRequestRepository.GetAll()
                .Where(x => x.CreationTime.Date > input.StartDate && x.CreationTime.Date < input.EndDate)
                .SelectMany(x => x.ShippingRequestDestinationCities.Select(x => new { dest = x.CityFk.DisplayName, x.CityId }))
                .GroupBy(x => new { x.dest, x.CityId })
                .OrderByDescending(x => x.Count())
                .Select(x => new GetRequestsHeapMapOutput
                {
                    CityType = "destination",
                    NumberOfRequests = x.Count(),
                    CityName = x.Key.dest,
                    CityId = x.Key.CityId
                }).Take(5).ToListAsync();

            return origins.Union(destinations).OrderByDescending(x=>x.NumberOfRequests).Take(5).ToList();

        }

        public async Task<GetInvoicesPaidBeforeDueDateOutput> GetInvoicesPaidBeforeDueDate()
        {
            DisableTenancyFilters();
            var invoices = _invoiceRepository.GetAll().Where(x => x.IsPaid && ((x.ConsiderConfirmationAndLoadingDates && x.ConfirmationDate.Value.Date.Month == Clock.Now.Month) ||
            (!x.ConsiderConfirmationAndLoadingDates && x.CreationTime.Date.Month == Clock.Now.Month)));

            var total = await invoices.CountAsync();
            var paidBeforeDueDate = total != 0 ? (await invoices.Where(x => x.PaymentDate < x.DueDate).CountAsync()) / total * 100 :0;
            var unPaidBeforeDueDate = total != 0 ? 100 - paidBeforeDueDate :0;

            return new GetInvoicesPaidBeforeDueDateOutput
            {
                totalInvoices = total,
                PaidInvoicesBeforeDueDatePercantage = paidBeforeDueDate,
                UnPaidInvoicesBeforeDueDatePercantage = unPaidBeforeDueDate
            };

        }

        public async Task<GetCostVsSellingTruckAggregationTripsOutput> GetCostVsSellingTruckAggregationTrips (DateRangeInput input)
        {
            return await ReturnCostSellingCalcPerMonth(input, false);
        }

        public async Task<GetCostVsSellingTruckAggregationTripsOutput> GetCostVsSellingSAASTrips(DateRangeInput input)
        {
            return await ReturnCostSellingCalcPerMonth(input, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter" 1: all, 2: SAAS , 3:Truck Aggregation></param>
        /// <returns></returns>
        public async Task<GetOverallAmountForAlltripsOutput> GetOverallAmountForAlltrips(int filter)
        {
            DisableTenancyFilters();
            var trips = await _shippingRequestTripRepository.GetAll()
                .Where(x => x.ActualPickupDate.HasValue)
                .WhereIf(filter == 3, x => (x.ShippingRequestFk != null && x.ShippingRequestFk.TenantId != x.ShippingRequestFk.CarrierTenantId) ||
                            (x.ShippingRequestFk == null && x.ShipperTenantId != x.CarrierTenantId))
                .WhereIf(filter == 2, x => (x.ShippingRequestFk != null && x.ShippingRequestFk.TenantId == x.ShippingRequestFk.CarrierTenantId) ||
                            (x.ShippingRequestFk == null && x.ShipperTenantId == x.CarrierTenantId))
                .Select(x => new
                {
                    x.TotalAmountWithCommission,
                    x.SubTotalAmount,
                })
                .ToListAsync();
            var selling = trips.Sum(x => x.TotalAmountWithCommission).Value;
            var cost = trips.Sum(x => x.SubTotalAmount).Value;
            return new GetOverallAmountForAlltripsOutput { Selling =selling, Cost = cost, Profit = selling - cost };
        }

        public async Task<GetOverallAmountForAlltripsOutput> GetTruckAggregationInvoicesCostAndSelling()
        {
            return await GetInvoicesCostAndSelling(false);
        }

        public async Task<GetOverallAmountForAlltripsOutput> GetSAASInvoicesCostAndSelling()
        {
            return await GetInvoicesCostAndSelling(true);
        }
        /// <summary>
        /// Filter: 1 = Normal , 2= SAAS, 3=HomeDelivery
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<List<GetUpcomingTripsOutput>> GetUpcomingTrips(int filter)
        {
            DisableTenancyFilters();
            var trips = await _shippingRequestTripRepository.GetAll().Where(x => x.StartTripDate.Date >= Clock.Now.Date && x.StartTripDate.Date < Clock.Now.AddDays(7).Date)
                .WhereIf(filter == 1, x => (x.ShippingRequestFk != null && x.ShippingRequestFk.TenantId != x.ShippingRequestFk.CarrierTenantId) ||
                            (x.ShippingRequestFk == null && x.ShipperTenantId != x.CarrierTenantId))
                .WhereIf(filter == 2, x => (x.ShippingRequestFk != null && x.ShippingRequestFk.TenantId == x.ShippingRequestFk.CarrierTenantId) ||
                            (x.ShippingRequestFk == null && x.ShipperTenantId == x.CarrierTenantId))
                .WhereIf(filter == 3, x => x.ShippingRequestTripFlag == ShippingRequestTripFlag.HomeDelivery)
                .Include(x=>x.ShippingRequestDestinationCities).ThenInclude(x=>x.CityFk)
                .Select(x => new
                {
                    requestOriginCity = x.ShippingRequestFk.OriginCityFk != null ? x.ShippingRequestFk.OriginCityFk.DisplayName : x.ShippingRequestFk.OriginFacility.Name + "- " + x.ShippingRequestFk.OriginFacility.CityFk.DisplayName,
                    tripOrigin = x.OriginCityFk != null ? x.OriginCityFk.DisplayName : x.OriginFacilityFk.Name + "- " + x.OriginFacilityFk.CityFk.DisplayName,
                    x.WaybillNumber, x.StartTripDate, x.ShippingRequestDestinationCities
                })
                .ToListAsync();

            var dto = new List<GetUpcomingTripsOutput>();

            foreach(var trip in trips)
            {
                var origin = trip.requestOriginCity != null ? trip.requestOriginCity : trip.tripOrigin;
                var destination = "";
                int index = 1;
                foreach(var dest in trip.ShippingRequestDestinationCities)
                {
                    if (index == 1)
                        destination = dest.CityFk.DisplayName;
                    else
                        destination = destination + "," + dest.CityFk.DisplayName;
                    index++;
                }

                dto.Add(new GetUpcomingTripsOutput { OrigiCity = origin, DestinationCity = destination, StartTripDate =trip.StartTripDate, WaybillNumber=trip.WaybillNumber.ToString() });
            }

            return dto;


        }


        public async Task<List<GetNeedsActionTripsAndRequestsOutput>> GetNeedsActionTripsAndRequests(DateRangeInput input)
        {
            DisableTenancyFilters();
            var trips = await (from point in _routePointRepository.GetAll() where
                               point.ShippingRequestTripFk.CreationTime > input.StartDate && point.ShippingRequestTripFk.CreationTime < input.EndDate &&
                               ((point.ShippingRequestTripFk.ShippingRequestId != null && !point.ShippingRequestTripFk.ShippingRequestFk.IsDeleted) || 
                               point.ShippingRequestTripFk.ShippingRequestId == null ) &&
                        point.ShippingRequestTripFk.Status == ShippingRequestTripStatus.DeliveredAndNeedsConfirmation
                        && point.PickingType == PickingType.Dropoff && !point.IsComplete && (!point.IsPodUploaded || !point.ShippingRequestTripFk.EndWorking.HasValue)
                        select new GetNeedsActionTripsAndRequestsOutput()
                        {
                            OriginCity = point.ShippingRequestTripFk.OriginFacilityFk.CityFk.DisplayName,
                            DestinationCity = point.ShippingRequestTripFk.ShippingRequestFk != null 
                            ? point.ShippingRequestTripFk.ShippingRequestFk.ShippingRequestDestinationCities.Select(x=>x.CityFk.DisplayName).ToList()
                            : point.ShippingRequestTripFk.ShippingRequestDestinationCities.Count() >0 
                            ? point.ShippingRequestTripFk.ShippingRequestDestinationCities.Select(x=>x.CityFk.DisplayName).ToList()
                            : new List<string> { point.FacilityFk.CityFk.DisplayName },
                            WaybillOrRequestReference = point.ShippingRequestTripFk.RouteType.HasValue
                                ? (point.ShippingRequestTripFk.RouteType == ShippingRequestRouteType.SingleDrop
                                    ? point.ShippingRequestTripFk.WaybillNumber.ToString()
                                    : point.WaybillNumber.ToString())
                                : (point.ShippingRequestTripFk.ShippingRequestFk.RouteTypeId ==
                                    ShippingRequestRouteType.SingleDrop
                                    ? point.ShippingRequestTripFk.WaybillNumber.ToString()
                                    : point.WaybillNumber.ToString()),
                            ActionName = !point.IsPodUploaded ?"POD" :"Receiver Code",
                        }).ToListAsync();


            var requestsNeedsAction =await _shippingRequestRepository.GetAll().Where(x => x.Status == ShippingRequestStatus.NeedsAction
            && x.CreationTime > input.StartDate && x.CreationTime.Date < input.EndDate).Select(x => new GetNeedsActionTripsAndRequestsOutput
            {
                ActionName = "Price Agreement",
                OriginCity = x.OriginCityFk != null ? x.OriginCityFk.DisplayName : $"{x.OriginFacility.Name}-{x.OriginFacility.CityFk.DisplayName}",
                DestinationCity = x.ShippingRequestDestinationCities.Select(x => x.CityFk.DisplayName).ToList(),
                WaybillOrRequestReference = x.ReferenceNumber
            }).ToListAsync() ;

            return trips.Union(requestsNeedsAction).ToList();
        }


        public async Task<GetNumberOfTruckAggregVsSAASTripsOutput> GetNumberOfTruckAggregVsSAASTrips (DateRangeInput input)
        {
            DisableTenancyFilters();
            var query = _shippingRequestTripRepository.GetAll().Where(x => x.CreationTime.Date > input.StartDate && x.CreationTime.Date < input.EndDate);

            var truckAggregTrips = (await query.Where(x=>x.ShippingRequestFk != null).Select(x => x.CreationTime).ToListAsync())
                .Select(x => new { CreationTime = x.Year + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month) }).GroupBy(x => x.CreationTime);


            var saasTrips = (await query.Where(x => x.ShippingRequestId == null).Select(x => x.CreationTime).ToListAsync())
                .Select(x => new { CreationTime = x.Year + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month) }).GroupBy(x => x.CreationTime);


            var dto = new GetNumberOfTruckAggregVsSAASTripsOutput
            {
                SAASTrips = new List<ChartCategoryPairedValuesDto>(),
                TruckAggregationTrips = new List<ChartCategoryPairedValuesDto>()
            };

            foreach (var monthWithYear in MonthsWithYearsInRange(input.StartDate, input.EndDate))
            {
                var truckAggTrips = truckAggregTrips.FirstOrDefault(x => x.Key == monthWithYear);
                if (truckAggTrips != null)
                {
                    dto.TruckAggregationTrips.Add(new ChartCategoryPairedValuesDto { X = truckAggTrips.Key, Y = truckAggTrips.Count() });
                }
                else
                {
                    dto.TruckAggregationTrips.Add(new ChartCategoryPairedValuesDto { X = monthWithYear, Y = 0 });
                }


                var saasTrip = saasTrips.FirstOrDefault(x => x.Key == monthWithYear);
                if (saasTrip != null)
                {
                    dto.SAASTrips.Add(new ChartCategoryPairedValuesDto { X = saasTrip.Key, Y = saasTrip.Count() });
                }
                else
                {
                    dto.SAASTrips.Add(new ChartCategoryPairedValuesDto { X = monthWithYear, Y = 0 });
                }
            }

            return dto;

        }

        public async Task<List<ChartCategoryPairedValuesDto>> GetNumberOfDedicatedTrips(DateRangeInput input)
        {
            await DisableTenancyFiltersIfTachyonDealer();
            var list = (await _shippingRequestTripRepository.GetAll()
                .Where(x => x.ShippingRequestFk.ShippingRequestFlag == ShippingRequestFlag.Dedicated && x.CreationTime.Date > input.StartDate && x.CreationTime < input.EndDate)
                .Select(x => x.CreationTime)
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


            // public async Task Get
            #region Helper
            private async Task<GetOverallAmountForAlltripsOutput> GetInvoicesCostAndSelling(bool isSaas)
        {
            DisableTenancyFilters();
            var trips = await _shippingRequestTripRepository.GetAll()
                .Where(x => x.ActualPickupDate.HasValue && x.CreationTime.Month == Clock.Now.Month)
                .WhereIf(!isSaas, x => (x.ShippingRequestFk != null && x.ShippingRequestFk.TenantId != x.ShippingRequestFk.CarrierTenantId) ||
                            (x.ShippingRequestFk == null && x.ShipperTenantId != x.CarrierTenantId))
                .WhereIf(isSaas, x => (x.ShippingRequestFk != null && x.ShippingRequestFk.TenantId == x.ShippingRequestFk.CarrierTenantId) ||
                            (x.ShippingRequestFk == null && x.ShipperTenantId == x.CarrierTenantId))
                .Select(x => new
                {
                    x.TotalAmountWithCommission,
                    x.SubTotalAmount,
                })
                .ToListAsync();
            var selling = trips.Sum(x => x.TotalAmountWithCommission).Value;
            var cost = trips.Sum(x => x.SubTotalAmount).Value;
            return new GetOverallAmountForAlltripsOutput { Selling = selling, Cost = cost, Profit = selling - cost };
        }

        private async Task<GetCostVsSellingTruckAggregationTripsOutput> ReturnCostSellingCalcPerMonth(DateRangeInput input, bool isForSaasTrip)
        {
            DisableTenancyFilters();
            var trips = (await _shippingRequestTripRepository.GetAll()
                .Where(x => x.ActualPickupDate.HasValue && x.CreationTime.Date > input.StartDate && x.CreationTime.Date < input.EndDate)
                .WhereIf(!isForSaasTrip,x=> (x.ShippingRequestFk != null && x.ShippingRequestFk.TenantId != x.ShippingRequestFk.CarrierTenantId) ||
                            (x.ShippingRequestFk == null && x.ShipperTenantId != x.CarrierTenantId))
                .WhereIf(isForSaasTrip, x => (x.ShippingRequestFk != null && x.ShippingRequestFk.TenantId == x.ShippingRequestFk.CarrierTenantId) ||
                            (x.ShippingRequestFk == null && x.ShipperTenantId == x.CarrierTenantId))
                .Select(x => new
                {
                    x.TotalAmountWithCommission,
                    x.SubTotalAmount,
                    x.CreationTime
                })
                .ToListAsync())
                .Select(x => new { x.TotalAmountWithCommission, x.SubTotalAmount, CreationTime = x.CreationTime.Year + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.CreationTime.Month) })
                .GroupBy(x => x.CreationTime);



            var dto = new GetCostVsSellingTruckAggregationTripsOutput()
            {
                Selling = new List<ChartCategoryPairedValuesDto>(),
                Cost = new List<ChartCategoryPairedValuesDto>(),
                Profit = new List<ChartCategoryPairedValuesDto>()
            };

            foreach (var monthWithYear in MonthsWithYearsInRange(input.StartDate, input.EndDate))
            {
                var trip = trips.FirstOrDefault(x => x.Key == monthWithYear);
                if (trip != null)
                {
                    var selling = Convert.ToInt32(trip.Sum(x => x.TotalAmountWithCommission).Value);
                    var cost = Convert.ToInt32(trip.Sum(x => x.SubTotalAmount).Value);
                    var profit = selling - cost;
                    dto.Selling.Add(new ChartCategoryPairedValuesDto { X = trip.Key, Y = selling });
                    dto.Cost.Add(new ChartCategoryPairedValuesDto { X = trip.Key, Y = cost });
                    dto.Profit.Add(new ChartCategoryPairedValuesDto { X = trip.Key, Y = profit });
                }
                else
                {
                    dto.Selling.Add(new ChartCategoryPairedValuesDto { X = monthWithYear, Y = 0 });
                    dto.Cost.Add(new ChartCategoryPairedValuesDto { X = monthWithYear, Y = 0 });
                    dto.Profit.Add(new ChartCategoryPairedValuesDto { X = monthWithYear, Y = 0 });
                }

            }
            return dto;
        }

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
