using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Dashboards.Carrier.Dto;
using TACHYON.Dashboards.Host.Dto;
using TACHYON.Dashboards.Shipper.Dto;
using TACHYON.Features;
using TACHYON.Goods.GoodCategories;
using TACHYON.Invoices;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.MultiTenancy;
using TACHYON.PriceOffers;
using TACHYON.PricePackages;
using TACHYON.Routs.RoutTypes;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.ShippingRequestVases;
using TACHYON.Trucks;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Dashboards.Carrier
{
    //[AbpAuthorize(AppPermissions.Pages_CarrierDashboard)]
    public class CarrierDashboardAppService : TACHYONAppServiceBase, ICarrierDashboardAppService
    {

        private readonly IRepository<User, long> _usersRepository;
        private readonly IRepository<Truck, long> _trucksRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<ShippingRequestVas, long> _shippingRequestVasRepository;
        private readonly IRepository<PriceOffer, long> _priceOfferRepository;
        private readonly IRepository<SubmitInvoice, long> _submitInvoiceRepository;
        private readonly IRepository<NormalPricePackage> _pricePackageRepository;

        public CarrierDashboardAppService(
             IRepository<User, long> usersRepository,
             IRepository<Truck, long> trucksRepository,
             IRepository<ShippingRequest, long> shippingRequestRepository,
             IRepository<ShippingRequestTrip> shippingRequestTripRepository,
             IRepository<ShippingRequestVas, long> shippingRequestVasRepository,
             IRepository<PriceOffer, long> priceOfferRepository,
             IRepository<SubmitInvoice, long> submitInvoiceRepository,
             IRepository<NormalPricePackage> pricePackageRepository)
        {
            _usersRepository = usersRepository;
            _trucksRepository = trucksRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _shippingRequestVasRepository = shippingRequestVasRepository;
            _priceOfferRepository = priceOfferRepository;
            _submitInvoiceRepository = submitInvoiceRepository;
            _pricePackageRepository = pricePackageRepository;
        }



        public async Task<List<ChartCategoryPairedValuesDto>> GetCompletedTripsCountPerMonth(GetDataByDateFilterInput input)
        {
            DisableTenancyFilters();



            //daily => default before 30 day
            if (input.DatePeriod == FilterDatePeriod.Daily)
            {
                return await GetCompletedTripsIfDaily(input);
            }
            else if (input.DatePeriod == FilterDatePeriod.Monthly)
            {
                return await GetCompletedTripsIfMonthly(input);
            }

            return new List<ChartCategoryPairedValuesDto>();
        }


        private async Task<List<ChartCategoryPairedValuesDto>> GetCompletedTripsIfMonthly(GetDataByDateFilterInput input)
        {
            var year = Clock.Now.Year;
            var trips = await _shippingRequestTripRepository
                .GetAll()
                .AsNoTracking()
                .Where(x => x.Status == ShippingRequestTripStatus.Delivered && x.CreationTime.Year == Clock.Now.Year)
                .Where(x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .ToListAsync();

            var result = trips
                        .GroupBy(r => r.CreationTime.Month)
                        .Select(g => new ChartCategoryPairedValuesDto
                        {
                            Y = g.Count(),

                            X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key)
                        }).ToList();

            return result;

        }


        private async Task<List<ChartCategoryPairedValuesDto>> GetCompletedTripsIfDaily(GetDataByDateFilterInput input)
        {

            var tripsDailyList = await _shippingRequestTripRepository.GetAll()
                .AsNoTracking()
            .Where(x => x.Status == ShippingRequestTripStatus.Delivered && x.CreationTime > Clock.Now.AddDays(-30))
            .Where(x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
            .ToListAsync();

            var result = tripsDailyList
                .GroupBy(r =>

                    r.CreationTime.Date
                )
                .Select(s => new ChartCategoryPairedValuesDto
                {
                    Y = s.Count(),
                    X = s.Key.ToString("dd/MM/yyyy"),

                }).OrderBy(r => r.Y).ToList();
            return result;

        }



        public async Task<ActivityItemsDto> GetDriversActivity()
        {
            DisableTenancyFilters();
            var query = _usersRepository
                .GetAll()
                .AsNoTracking()
                .Where(r => r.TenantId == AbpSession.TenantId)
                .Where(x => x.IsDriver);

            var inTransitTrucks = await _shippingRequestTripRepository
                .GetAll()
                .Where(x => x.Status == ShippingRequestTripStatus.InTransit)
                .Where(x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .Select(x => x.AssignedDriverUserId)
                .Distinct()
                .CountAsync();

            var activeItems = await query.CountAsync(r => r.IsActive); // todo convert to enum 
            var notActiveItems = await query.CountAsync(r => !r.IsActive);


            return new ActivityItemsDto()
            {
                ActiveItems = activeItems,
                NotActiveItems = notActiveItems,
                InTransitItems = inTransitTrucks,
                TotalItemsCount = activeItems + notActiveItems
            };
        }

        public async Task<ActivityItemsDto> GetTrucksActivity()
        {
            DisableTenancyFilters();
            var query = _trucksRepository
                .GetAll()
                .AsNoTracking()
                .Where(r => r.TenantId == AbpSession.TenantId);

            var inTransitTrucks = await _shippingRequestTripRepository
                .GetAll()
                .Where(x => x.Status == ShippingRequestTripStatus.InTransit)
                .Where(x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .Select(x => x.AssignedTruckId)
                .Distinct()
                .CountAsync();

            var activeItems = await query.CountAsync(r => r.TruckStatusId == 1); // todo convert to enum 
            var notActiveItems = await query.CountAsync(r => r.TruckStatusId == 2);



            return new ActivityItemsDto()
            {
                ActiveItems = activeItems,
                NotActiveItems = notActiveItems,
                InTransitItems = inTransitTrucks,
                TotalItemsCount = activeItems + notActiveItems
            };
        }

        public async Task<List<MostTenantWorksListDto>> GetMostWorkedWithShippers()
        {
       
                DisableTenancyFilters();

                var trips = await _shippingRequestTripRepository
                     .GetAll()
                     .Include(r => r.ShippingRequestFk)
                     .ThenInclude(x => x.Tenant)
                     .AsNoTracking()
                     .Where(t => t.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId )
                     .Select
                     (
                         x => new
                         {
                             shipperId = x.ShippingRequestFk.Tenant.Id,
                             shipperName = x.ShippingRequestFk.Tenant.TenancyName,
                             shipperRating = x.ShippingRequestFk.Tenant.Rate
                         }
                     )
                     .ToListAsync();

                return trips
                    .GroupBy(x => x.shipperId)
                    .Select(
                        g => new MostTenantWorksListDto()
                        {
                            Id = g.Key,
                            Name = g.FirstOrDefault()?.shipperName,
                            Rating = g.FirstOrDefault()?.shipperRating,
                            NumberOfTrips = g.Count(),
                        })
                 .OrderByDescending(x => x.NumberOfTrips)
                 .Take(5)
                 .ToList();


            


        }

        public async Task<List<ChartCategoryPairedValuesDto>> GetMostVasesUsedByShippers()
        {
            DisableTenancyFilters();

             var shippingRequestVases =  await _shippingRequestVasRepository
                    .GetAll()
                    .AsNoTracking()
                    .Include(r => r.VasFk).ThenInclude(x=> x.Translations)
                    .Where(x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                    .ToListAsync();
                var mostVasesUsed = (from srVas in shippingRequestVases
                        group srVas by srVas.VasId into vasGroup
                        let vasFk = vasGroup.Select(x=> x.VasFk).FirstOrDefault(x=> x.Id == vasGroup.Key)
                        let vasTranslation = vasFk.Translations.FirstOrDefault(x=> x.Language.Contains(CultureInfo.CurrentUICulture.Name))
                        select new ChartCategoryPairedValuesDto()
                        {
                            X = vasTranslation != null ? vasTranslation.DisplayName : vasFk.Key,
                            Y = vasGroup.Count()
                        }).Distinct()
                    .OrderByDescending(r => r.Y)
                .Take(5)
                .ToList();

             return mostVasesUsed;
        }

        public async Task<AcceptedAndRejectedRequestsListDto> GetAcceptedAndRejectedRequests()
        {
            DisableTenancyFilters();


            var query = _priceOfferRepository
                .GetAll()
                .AsNoTracking()
                .Where(x => x.CreationTime.Year == Clock.Now.Year)
                .Where(x => x.TenantId == AbpSession.TenantId)
                .Select(x => new
                {
                    x.Status,
                    x.CreationTime.Month
                });

            var accepted = await query
                .Where(x => x.Status == PriceOfferStatus.Accepted)
                .GroupBy(x => x.Month)
                .Select(g => new ChartCategoryPairedValuesDto
                {
                    X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                    Y = g.Count()
                })
                .ToListAsync();

            var rejected = await query
                .Where(x => x.Status == PriceOfferStatus.Rejected)
                .GroupBy(x => x.Month)
                .Select(g => new ChartCategoryPairedValuesDto
                {
                    X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                    Y = g.Count()
                })
                .ToListAsync();

            return new AcceptedAndRejectedRequestsListDto
            {
                AcceptedOffers = accepted,
                RejectedOffers = rejected
            };
        }

        public async Task<GetCarrierInvoicesDetailsOutput> GetCarrierInvoicesDetails()
        {
            DisableTenancyFilters();


            var query = _submitInvoiceRepository
                .GetAll()
                .AsNoTracking()
                .Where(x => x.TenantId == AbpSession.TenantId);

            var paid = (await query
                    .Where(x => x.Status == SubmitInvoiceStatus.Paid)
                    .ToListAsync())
                .GroupBy(x => x.CreationTime.Date.Month)
                    .Select(g => new ChartCategoryPairedValuesDto
                    {
                        X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                        Y = g.Count()
                    })
                    .OrderBy(x => x.X)
                    .ToList();


            var claimed = (await query
               .Where(x => x.Status == SubmitInvoiceStatus.Claim)
               .ToListAsync())
           .GroupBy(x => x.CreationTime.Date.Month)
               .Select(g => new ChartCategoryPairedValuesDto
               {
                   X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                   Y = g.Count()
               })
               .OrderBy(x => x.X)
               .ToList();



            var newInvoices = (await query
                 .Where(x => x.Status == SubmitInvoiceStatus.New)
                 .ToListAsync())
             .GroupBy(x => x.CreationTime.Date.Month)
                 .Select(g => new ChartCategoryPairedValuesDto
                 {
                     X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                     Y = g.Count()
                 })
                 .OrderBy(x => x.X)
                 .ToList();


            var total = (await query
                    .ToListAsync())
                    .GroupBy(x => x.CreationTime.Date.Month)
                    .Select(g => new ChartCategoryPairedValuesDto
                    {
                        X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                        Y = g.Count()
                    })
                    .OrderBy(x => x.X)
                    .ToList();


            return new GetCarrierInvoicesDetailsOutput
            {
                PaidInvoices = paid,
                NewInvoices = newInvoices,
                Total = total,
                Claimed = claimed
            };
        }


        public async Task<List<ChartCategoryPairedValuesDto>> GetMostPricePackageByShippers()
        {
            //DisableTenancyFilters();

            return (await _pricePackageRepository
                    .GetAll()
                    .AsNoTracking()
                    //.Include(r => r.Tenant)
                    .ToListAsync())
                .GroupBy(r => new { r.DisplayName })
                .Select(vas => new ChartCategoryPairedValuesDto()
                {
                    X = vas.Key.DisplayName,
                    Y = vas.Count()
                }).Distinct()
                .OrderByDescending(r => r.Y)
                .Take(5)
                .ToList();
        }



    }

    public class GetCarrierInvoicesDetailsOutput
    {
        public List<ChartCategoryPairedValuesDto> PaidInvoices { get; set; }
        public List<ChartCategoryPairedValuesDto> NewInvoices { get; set; }
        public List<ChartCategoryPairedValuesDto> Total { get; set; }
        public List<ChartCategoryPairedValuesDto> Claimed { get; set; }
    }
}