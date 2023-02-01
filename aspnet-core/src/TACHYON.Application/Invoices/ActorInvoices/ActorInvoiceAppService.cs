using Abp.Application.Features;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Localization.Sources;
using AutoMapper.QueryableExtensions;
using Castle.Core.Internal;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Features;
using TACHYON.Invoices.ActorInvoices.Dto;
using TACHYON.Invoices.Dto;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Invoices.ActorInvoices
{
    [AbpAuthorize(permissions: AppPermissions.Pages_Administration_ActorsInvoice)]
    public class ActorInvoiceAppService : TACHYONAppServiceBase, IApplicationService
    {

        private readonly IRepository<ActorInvoice, long> _actorInvoiceRepository;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;
        private readonly IRepository<ShippingRequestTrip> _tripRepository;

        public ActorInvoiceAppService(
            IRepository<ActorInvoice, long> actorInvoiceRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IRepository<ShippingRequestTrip> tripRepository)
        {
            _actorInvoiceRepository = actorInvoiceRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _tripRepository = tripRepository;
        }


        public async Task<LoadResult> GetAll(string filter)
        {
            
            bool isCmsEnabled = await FeatureChecker.IsEnabledAsync(AppFeatures.CMS);
            
            List<long> userOrganizationUnits = null;
            if (isCmsEnabled)
            {
                userOrganizationUnits = await _userOrganizationUnitRepository.GetAll().Where(x => x.UserId == AbpSession.UserId)
                    .Select(x => x.OrganizationUnitId).ToListAsync();
            }
            
            var query = _actorInvoiceRepository
                .GetAll()
                .WhereIf(isCmsEnabled && !userOrganizationUnits.IsNullOrEmpty(),
                    x=> x.ShipperActorId.HasValue && userOrganizationUnits.Contains(x.ShipperActorFk.OrganizationUnitId))
                .ProjectTo<ActorInvoiceListDto>(AutoMapperConfigurationProvider)
                .AsNoTracking();
            if (!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                DisableTenancyFilters();
            }

            return await LoadResultAsync<ActorInvoiceListDto>(query, filter);
        }

        public async Task<bool> MakePaid(long invoiceId)
        {
            var Invoice = await GetActorInvoice(invoiceId);
            if (Invoice != null && !Invoice.IsPaid)
            {
                Invoice.IsPaid = true;
                return true;
            }
            return false;
        }

        public async Task MakeUnPaid(long invoiceId)
        {
            var Invoice = await GetActorInvoice(invoiceId);
            if (Invoice != null && Invoice.IsPaid)
            {
                Invoice.IsPaid = false;
            }
        }


        private async Task<ActorInvoice> GetActorInvoice(long id)
        {
            await DisableDraftedFilterIfTachyonDealerOrHost();
            return await _actorInvoiceRepository.FirstOrDefaultAsync(id);
        }

        public async Task<InvoiceInfoDto> GetActorInvoiceDetails(long invoiceId)
        {
            var submitInvoiceDetails = await _actorInvoiceRepository.GetAll()
                .Where(x => x.Id == invoiceId).Select(x => new InvoiceInfoDto()
                {
                    SubTotalAmount = x.SubTotalAmount,
                    VatAmount = x.VatAmount,
                    TotalAmount = x.TotalAmount,
                    TaxVat = x.TaxVat
                }).FirstOrDefaultAsync();

            submitInvoiceDetails.Items = await GetActorInvoiceItems(invoiceId);
            return submitInvoiceDetails;
        }
        private async Task<List<InvoiceItemDto>> GetActorInvoiceItems (long invoiceId)
        {

            
            var invoiceTrips = await (from trip in _tripRepository.GetAll().AsNoTracking()
                where trip.ActorInvoiceId == invoiceId
                select new
                {
                    trip.Id,VasCount = trip.ShippingRequestTripVases.Count,
                    SubTotalAmount = trip.ShippingRequestFk.ActorShipperPrice.SubTotalAmountWithCommission.Value,VatAmount = trip.ShippingRequestFk.ActorShipperPrice.VatAmountWithCommission.Value,
                    WaybillNumber = trip.WaybillNumber.ToString(),
                    TruckType = trip.AssignedTruckFk.TrucksTypeFk.Translations.FirstOrDefault(t=> t.Language.Contains(CultureInfo.CurrentUICulture.Name)).DisplayName,
                    Source = trip.ShippingRequestFk.OriginCityFk.Translations.FirstOrDefault(t=> t.Language.Contains(CultureInfo.CurrentUICulture.Name)).TranslatedDisplayName,
                    Destination = trip.DestinationFacilityFk.CityFk.Translations.FirstOrDefault(t=> t.Language.Contains(CultureInfo.CurrentUICulture.Name)).TranslatedDisplayName,
                    DateWork = trip.EndWorking.HasValue ? trip.EndWorking.Value.ToString("dd MMM, yyyy") : string.Empty,
                    Remarks = trip.ShippingRequestFk.RouteTypeId == ShippingRequestRouteType.MultipleDrops ?
                        LocalizationSource.GetString("TotalOfDrop", trip.ShippingRequestFk.NumberOfDrops) : string.Empty,
                    Vases = trip.ShippingRequestTripVases.Select(x=> new
                    {
                        VasName = x.ShippingRequestVasFk.VasFk.Key, SubTotalAmount = x.ShippingRequestVasFk.ActorShipperPrice.SubTotalAmountWithCommission.Value,
                        VatAmount = x.ShippingRequestVasFk.ActorShipperPrice.VatAmountWithCommission.Value, 
                        TotalAmount = x.ShippingRequestVasFk.ActorShipperPrice.TotalAmountWithCommission.Value,
                        Remarks = x.Quantity > 1 ? x.Quantity.ToString() : string.Empty
                    })
                    
                }).ToListAsync();
            
            
            var totalItems = invoiceTrips.Count +
                             invoiceTrips.Sum(x=> x.VasCount);
            
            int sequence = 1;
            List<InvoiceItemDto> items = new List<InvoiceItemDto>();

            foreach (var trip in invoiceTrips)
            {
                int vasCounter = trip.VasCount > 1 ? 1 : 0;

                var invoiceItem = new InvoiceItemDto
                {
                    Sequence = $"{sequence}/{totalItems}",
                    SubTotalAmount = trip.SubTotalAmount,
                    VatAmount = trip.VatAmount,
                    TotalAmount = trip.SubTotalAmount + trip.VatAmount,
                    WayBillNumber = trip.WaybillNumber,
                    TruckType = trip.TruckType,
                    Source = trip.Source,
                    Destination = trip.Destination,
                    DateWork = trip.DateWork,
                    Remarks = trip.Remarks
                };

                if (!trip.Vases.IsNullOrEmpty())
                {
                    invoiceItem.TotalAmount += trip.Vases.Sum(i => i.TotalAmount);
                }
                
                items.Add(invoiceItem);
                
                sequence++;
                
                

                foreach (var vas in trip.Vases)
                {
                    string waybillNumber;
                    if (vasCounter == 0)
                    {
                        waybillNumber = $"{trip.WaybillNumber}VAS";
                    }
                    else
                    {
                        waybillNumber = $"{trip.WaybillNumber}VAS{vasCounter}";
                        vasCounter++;
                    }

                    var item = new InvoiceItemDto
                    {
                        Sequence = $"{sequence}/{totalItems}",
                        SubTotalAmount = vas.SubTotalAmount,
                        VatAmount = vas.VatAmount,
                        TotalAmount = vas.TotalAmount,
                        WayBillNumber = waybillNumber,
                        TruckType = L("InvoiceVasType", vas.VasName),
                        Source = "-",
                        Destination = "-",
                        DateWork = "-",
                        Remarks = vas.Remarks
                    };
                    items.Add(item);

                    sequence++;
                }

            }
            
            return items;
        }
    }


}
