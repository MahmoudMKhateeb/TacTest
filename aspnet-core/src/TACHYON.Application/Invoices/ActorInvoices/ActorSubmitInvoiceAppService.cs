using Abp.Application.Features;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Localization.Sources;
using Abp.Timing;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using IdentityServer4.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Cities.Dtos;
using TACHYON.Common;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Invoices.ActorInvoices.Dto;
using TACHYON.Invoices.Balances;
using TACHYON.Invoices.Dto;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.Invoices.SubmitInvoices.Dto;
using TACHYON.Invoices.Transactions;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Invoices.ActorInvoices
{

    [AbpAuthorize]
    [RequiresFeature(AppFeatures.CarrierClients, AppFeatures.TachyonDealer)]

    public class ActorSubmitInvoiceAppService : TACHYONAppServiceBase, IApplicationService
    {
        private readonly IRepository<ActorSubmitInvoice, long> _actorSubmitInvoiceRepository;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;
        private readonly CommonManager _commonManager;
        private readonly IRepository<ShippingRequestTrip> _tripRepository;

        public ActorSubmitInvoiceAppService(
            IRepository<ActorSubmitInvoice, long> actorSubmitInvoiceRepository,
            CommonManager commonManager,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IRepository<ShippingRequestTrip> tripRepository)
        {
            _actorSubmitInvoiceRepository = actorSubmitInvoiceRepository;
            _commonManager = commonManager;
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
            
            
            var query = _actorSubmitInvoiceRepository
                .GetAll()
                .WhereIf(isCmsEnabled && !userOrganizationUnits.IsNullOrEmpty(),
                    x=> x.CarrierActorId.HasValue && userOrganizationUnits.Contains(x.CarrierActorFk.OrganizationUnitId))
                .ProjectTo<ActorSubmitInvoiceListDto>(AutoMapperConfigurationProvider)
                .AsNoTracking();
            if (!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                DisableTenancyFilters();
            }

            return await LoadResultAsync<ActorSubmitInvoiceListDto>(query, filter);
        }

        public async Task MakeActorSubmitInvoicePaid(long SubmitinvoiceId)
        {
           // CheckIfCanAccessService(true, AppFeatures.TachyonDealer);

            var Invoice = await GetActorSubmitInvoice(SubmitinvoiceId);
            if (Invoice != null && Invoice.Status != SubmitInvoiceStatus.Paid)
            {
                await _commonManager.ExecuteMethodIfHostOrTenantUsers(() => {
                    Invoice.Status = SubmitInvoiceStatus.Paid;
                    return Task.CompletedTask;
                });
            }
        }

        public async Task MakeActorSubmitInvoiceUnPaid(long SubmitinvoiceId)
        {

            var Invoice = await GetActorSubmitInvoice(SubmitinvoiceId);
            if (Invoice != null && Invoice.Status != SubmitInvoiceStatus.UnPaid)
            {
                Invoice.Status = SubmitInvoiceStatus.UnPaid;
            }
        }

        public async Task Claim(SubmitInvoiceClaimCreateInput Input)
        {
            var submit = await GetActorSubmitInvoice(Input.Id);
            submit.DueDate = Clock.Now;

            if (submit.Status == SubmitInvoiceStatus.Claim || submit.Status == SubmitInvoiceStatus.Accepted) return;

            var document = await _commonManager.UploadDocumentAsBase64(ObjectMapper.Map<DocumentUpload>(Input), AbpSession.TenantId);
            submit.Status = SubmitInvoiceStatus.Claim;
            ObjectMapper.Map(document, submit);

        }

        public async Task<FileDto> GetFileDto(long GroupId)
        {
            DisableTenancyFilters();
            var documentFile = await _actorSubmitInvoiceRepository.FirstOrDefaultAsync(g =>
                g.Id == GroupId && g.Status != SubmitInvoices.SubmitInvoiceStatus.New);
            if (documentFile == null)
                throw new UserFriendlyException(L("TheRequestNotFound"));

            return await _commonManager.GetDocument(ObjectMapper.Map<IHasDocument>(documentFile));
        }


        private async Task<ActorSubmitInvoice> GetActorSubmitInvoice(long id)
        {
            DisableTenancyFilters();
            return await _actorSubmitInvoiceRepository.GetAll()
                .Include(x => x.Tenant)
                .WhereIf(AbpSession.TenantId.HasValue && !await IsEnabledAsync(AppFeatures.TachyonDealer), e => e.TenantId == AbpSession.TenantId.Value)
                .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), e => true)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<SubmitInvoiceInfoDto> GetActorSubmitInvoiceDetails(long submitInvoiceId)
        {
            var submitInvoiceDetails = await _actorSubmitInvoiceRepository.GetAll()
                .Where(x => x.Id == submitInvoiceId).Select(x => new SubmitInvoiceInfoDto()
                {
                    SubTotalAmount = x.SubTotalAmount,
                    VatAmount = x.VatAmount,
                    TotalAmount = x.TotalAmount,
                    TaxVat = x.TaxVat
                }).FirstOrDefaultAsync();

            submitInvoiceDetails.Items = await GetActorSubmitInvoiceItems(submitInvoiceId);
            return submitInvoiceDetails;
        }
        private async Task<List<InvoiceItemDto>> GetActorSubmitInvoiceItems (long submitInvoiceId)
        {

            
            var submitInvoiceTrips = await (from trip in _tripRepository.GetAll().AsNoTracking()
                where trip.ActorSubmitInvoiceId == submitInvoiceId
                select  new
                {
                    trip.Id,
                    VasCount = trip.ShippingRequestTripVases.Count,
                    SubTotalAmount = trip.ShippingRequestId.HasValue? trip.ShippingRequestFk.ActorCarrierPrice.SubTotalAmount.Value : trip.ActorCarrierPrice.SubTotalAmount.Value,
                    VatAmount = trip.ShippingRequestId.HasValue ? trip.ShippingRequestFk.ActorCarrierPrice.VatAmount.Value : trip.ActorCarrierPrice.VatAmount.Value,
                    WaybillNumber = trip.WaybillNumber.ToString(),
                    TruckType = trip.AssignedTruckFk.TrucksTypeFk.Translations.FirstOrDefault(t=> t.Language.Contains(CultureInfo.CurrentUICulture.Name)).DisplayName,
                    Source = trip.ShippingRequestId.HasValue ? trip.ShippingRequestFk.OriginCityFk.Translations.FirstOrDefault(t=> t.Language.Contains(CultureInfo.CurrentUICulture.Name)).TranslatedDisplayName : trip.OriginFacilityFk.CityFk.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).TranslatedDisplayName,
                    Destination = trip.ShippingRequestId.HasValue ? trip.ShippingRequestFk.DestinationCityFk.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).TranslatedDisplayName : trip.DestinationFacilityFk.CityFk.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).TranslatedDisplayName,
                    DateWork = trip.EndWorking.HasValue ? trip.EndWorking.Value.ToString("dd MMM, yyyy") : string.Empty,
                    Remarks = trip.ShippingRequestId.HasValue ? (trip.ShippingRequestFk.RouteTypeId == ShippingRequestRouteType.MultipleDrops
                        ? LocalizationSource.GetString("TotalOfDrop", trip.ShippingRequestFk.NumberOfDrops)
                        : string.Empty) : (trip.RouteType == ShippingRequestRouteType.MultipleDrops
                        ? LocalizationSource.GetString("TotalOfDrop", trip.NumberOfDrops)
                        : string.Empty),
                    Vases = trip.ShippingRequestTripVases.Select(x=> new
                    {
                        VasName = x.ShippingRequestTripFk.ShippingRequestId != null
                        ? x.ShippingRequestVasFk.VasFk.Key
                        : x.VasFk.Key,
                        SubTotalAmount = x.ShippingRequestTripFk.ShippingRequestId != null 
                        ? x.ShippingRequestVasFk.ActorCarrierPrice.SubTotalAmount.Value
                        : x.SubTotalAmount.Value,
                        VatAmount = x.ShippingRequestTripFk.ShippingRequestId != null
                        ?x.ShippingRequestVasFk.ActorCarrierPrice.VatAmount.Value
                        : x.VatAmount, 
                        TotalAmount = x.ShippingRequestTripFk.ShippingRequestId != null
                        ?x.ShippingRequestVasFk.ActorCarrierPrice.VatAmount.Value + x.ShippingRequestVasFk.ActorCarrierPrice.SubTotalAmount.Value
                        : x.VatAmount.Value + x.SubTotalAmount.Value,
                        Remarks = x.Quantity > 1 ? x.Quantity.ToString() : string.Empty
                    })
                    
                }).ToListAsync();
            
            
            var totalItems = submitInvoiceTrips.Count +
                             submitInvoiceTrips.Sum(x=> x.VasCount);
            
            int sequence = 1;
            List<InvoiceItemDto> items = new List<InvoiceItemDto>();

            foreach (var trip in submitInvoiceTrips)
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
                        VatAmount = vas.VatAmount.Value,
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
