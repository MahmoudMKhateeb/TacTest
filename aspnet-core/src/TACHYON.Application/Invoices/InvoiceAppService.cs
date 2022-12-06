using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Threading;
using Abp.Timing;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Cities.Dtos;
using TACHYON.Common;
using TACHYON.Configuration;
using TACHYON.DataExporting;
using TACHYON.DataFilters;
using TACHYON.DedicatedDynamicInvoices.DedicatedDynamicInvoiceItems;
using TACHYON.DedicatedDynamicInvoices.Dtos;
using TACHYON.DedicatedInvoices;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Documents.DocumentTypes;
using TACHYON.Dto;
using TACHYON.DynamicInvoices;
using TACHYON.DynamicInvoices.DynamicInvoiceItems;
using TACHYON.Exporting;
using TACHYON.Features;
using TACHYON.Invoices.ActorInvoices;
using TACHYON.Invoices.ActorInvoices.Dto;
using TACHYON.Invoices.Balances;
using TACHYON.Invoices.Dto;
using TACHYON.Invoices.Periods;
using TACHYON.Invoices.Transactions;
using TACHYON.MultiTenancy;
using TACHYON.Penalties;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.ShippingRequestVases;
using TACHYON.Storage;
using TACHYON.Trucks.TrucksTypes.Dtos;
using TACHYON.Url;

namespace TACHYON.Invoices
{
    [AbpAuthorize(AppPermissions.Pages_Invoices)]
    public class InvoiceAppService : TACHYONAppServiceBase, IInvoiceAppService
    {
        private readonly IRepository<Invoice, long> _invoiceRepository;
        private readonly CommonManager _commonManager;
        private readonly BalanceManager _balanceManager;
        private readonly UserManager _userManager;
        private readonly InvoiceManager _invoiceManager;
        private readonly TransactionManager _transactionManager;
        private readonly IRepository<ShippingRequestVas, long> _shippingRequestVasesRepository;
        private readonly IExcelExporterManager<InvoiceListDto> _excelExporterManager;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly IExcelExporterManager<InvoiceItemDto> _excelExporterInvoiceItemManager;
        private readonly IWebUrlService _webUrlService;
        private readonly PdfExporterBase _pdfExporterBase;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly ISettingManager _settingManager;
        private readonly IRepository<DynamicInvoice, long> _dynamicInvoiceRepository;
        private readonly IRepository<ActorInvoice, long> _actorInvoiceRepository;
        private readonly DbBinaryObjectManager _binaryObjectManager;
        private readonly IRepository<DedicatedDynamicInvoice, long> _dedicatedDynamicInvoiceRepository;


        public InvoiceAppService(
            IRepository<Invoice, long> invoiceRepository,
            CommonManager commonManager,
            BalanceManager balanceManager,
            UserManager userManager,
            InvoiceManager invoiceManager,
            TransactionManager transactionManager,
            IExcelExporterManager<InvoiceListDto> excelExporterManager,
            IRepository<ShippingRequestVas, long> shippingRequestVasesRepository,
            IRepository<DocumentFile, Guid> documentFileRepository,
            IExcelExporterManager<InvoiceItemDto> excelExporterInvoiceItemManager,
             IWebUrlService webUrlService, PdfExporterBase pdfExporterBase, IRepository<ShippingRequestTrip> shippingRequestTripRepository, ISettingManager settingManager, IRepository<DynamicInvoice, long> dynamicInvoiceRepository,
            IRepository<ActorInvoice, long> actorInvoiceRepository,
            DbBinaryObjectManager binaryObjectManager, IRepository<DedicatedDynamicInvoice, long> dedicatedDynamicInvoiceRepository)

        {
            _invoiceRepository = invoiceRepository;
            _commonManager = commonManager;
            _balanceManager = balanceManager;
            _userManager = userManager;
            _invoiceManager = invoiceManager;
            _transactionManager = transactionManager;
            _shippingRequestVasesRepository = shippingRequestVasesRepository;
            _excelExporterManager = excelExporterManager;
            _documentFileRepository = documentFileRepository;
            _excelExporterInvoiceItemManager = excelExporterInvoiceItemManager;
            _webUrlService = webUrlService;
            _pdfExporterBase = pdfExporterBase;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _settingManager = settingManager;
            _dynamicInvoiceRepository = dynamicInvoiceRepository;
            _actorInvoiceRepository = actorInvoiceRepository;
            _binaryObjectManager = binaryObjectManager;
            _dedicatedDynamicInvoiceRepository = dedicatedDynamicInvoiceRepository;
        }


        public async Task<LoadResult> GetAll(string filter, InvoiceSearchInputDto input)

        {
            var query = _invoiceRepository
                .GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.ContainerNumber) , x=>x.Trips.Any(y=>y.ShippingRequestTripFK.ContainerNumber == input.ContainerNumber) ||
                x.Penalties.Any(y=>y.ShippingRequestTripFK.ContainerNumber == input.ContainerNumber))
                .WhereIf(input.WaybillOrSubWaybillNumber !=null, x => x.Trips.Any(y => y.ShippingRequestTripFK.WaybillNumber == input.WaybillOrSubWaybillNumber) ||
                x.Penalties.Any(y => y.ShippingRequestTripFK.WaybillNumber == input.WaybillOrSubWaybillNumber) ||
                  x.Trips.Any(y => y.ShippingRequestTripFK.RoutPoints.Any(w=>w.WaybillNumber == input.WaybillOrSubWaybillNumber)) ||
                x.Penalties.Any(y => y.ShippingRequestTripFK.RoutPoints.Any(w=>w.WaybillNumber == input.WaybillOrSubWaybillNumber)))
                .WhereIf(input.PaymentDate !=null, x=>x.PaymentDate!=null && x.PaymentDate.Value.Date==input.PaymentDate.Value.Date)
                .WhereIf(! string.IsNullOrWhiteSpace(input.AccountNumber), x=> x.Tenant.AccountNumber == input.AccountNumber)
                .ProjectTo<InvoiceListDto>(AutoMapperConfigurationProvider)
                .AsNoTracking();
            if (!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                DisableTenancyFilters();
                CurrentUnitOfWork.DisableFilter(TACHYONDataFilters.HaveInvoiceStatus);
            }

            return await LoadResultAsync<InvoiceListDto>(query, filter);
        }

        public async Task<InvoiceInfoDto> GetById(EntityDto input)
        {
            var invoice = await GetInvoiceInfo(input.Id);
            List<InvoiceItemDto> Items = GetInvoiceItems(invoice);
            var invoiceDto = ObjectMapper.Map<InvoiceInfoDto>(invoice);
            var Admin = await _userManager.GetAdminByTenantIdAsync(invoice.TenantId);
            invoiceDto.Items = Items;
            invoiceDto.Phone = Admin.PhoneNumber;
            invoiceDto.Email = Admin.EmailAddress;
            DisableTenancyFilters();
            var documnet =
                await _documentFileRepository.FirstOrDefaultAsync(x =>
                    x.TenantId == invoice.TenantId && x.DocumentTypeId == 14);
            if (documnet != null) invoiceDto.CR = documnet.Number;
            return invoiceDto;
        }

        private async Task<Invoice> GetInvoiceInfo(long invoiceId)
        {
            DisableTenancyFilters();
            var invoice = await _invoiceRepository
                .GetAll()
                .Include(i => i.InvoicePeriodsFK)
                .Include(i => i.Tenant)
                .Include(i => i.Trips)
                .ThenInclude(r => r.ShippingRequestTripFK)
                .ThenInclude(r => r.ShippingRequestTripVases)
                .ThenInclude(v => v.ShippingRequestVasFk)
                .ThenInclude(v => v.VasFk)
                .Include(i => i.Trips)
                .ThenInclude(r => r.ShippingRequestTripFK)
                .ThenInclude(i => i.ShippingRequestFk)
                .ThenInclude(r => r.OriginCityFk)
                .ThenInclude(r => r.Translations)
                .Include(i => i.Trips)
                .ThenInclude(r => r.ShippingRequestTripFK)
                .ThenInclude(i => i.ShippingRequestFk)
                .Include(i => i.Trips)
                .ThenInclude(r => r.ShippingRequestTripFK)
                .ThenInclude(r => r.AssignedTruckFk)
                .Include(i => i.Trips)
                .ThenInclude(r => r.ShippingRequestTripFK)
                .ThenInclude(r => r.AssignedTruckFk)
                .ThenInclude(r => r.TrucksTypeFk)
                .ThenInclude(r => r.Translations)
                .Include(i => i.Trips)
                .ThenInclude(r => r.ShippingRequestTripFK)
                .ThenInclude(r => r.DestinationFacilityFk)
                .ThenInclude(r => r.CityFk)
                .Include(i => i.Trips)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);
            if (invoice == null) throw new UserFriendlyException(L("TheInvoiceNotFound"));

            return invoice;
        }
        private async Task<ActorInvoice> GetActorInvoiceInfo(long actorInvoiceId)
        {
            DisableTenancyFilters();
            var actorInvoice = await _actorInvoiceRepository
                .GetAll()
                .Include(i => i.Trips)
                .ThenInclude(r => r.ShippingRequestTripVases)
                .ThenInclude(v => v.ShippingRequestVasFk)
                .ThenInclude(v => v.VasFk)
                .Include(i => i.Trips)
                .ThenInclude(r => r.ShippingRequestFk)
                .ThenInclude(r => r.ActorShipperPrice)
                .Include(i => i.Trips)
                .ThenInclude(i => i.ShippingRequestFk)
                .ThenInclude(r => r.OriginCityFk)
                .ThenInclude(r => r.Translations)
                .Include(i => i.Trips)
                .ThenInclude(i => i.ShippingRequestFk)
                .ThenInclude(r => r.ShippingRequestDestinationCities)
                .Include(i => i.Trips)
                .ThenInclude(r => r.AssignedTruckFk)
                .Include(i => i.Trips)
                .ThenInclude(r => r.AssignedTruckFk)
                .ThenInclude(r => r.TrucksTypeFk)
                .ThenInclude(r => r.Translations)
                .Include(i => i.Trips)
                .ThenInclude(i => i.ShippingRequestTripVases)
                .ThenInclude(x=> x.ShippingRequestVasFk)
                .ThenInclude(x=> x.ActorShipperPrice)
                .FirstOrDefaultAsync(i => i.Id == actorInvoiceId);
            if (actorInvoice == null) throw new UserFriendlyException(L("TheInvoiceNotFound"));

            return actorInvoice;
        }



        private async Task<Invoice> GetPenaltyInvoiceInfo(long penaltyInvoiceId)
        {
            DisableTenancyFilters();
            var invoice = await _invoiceRepository
                .GetAll()
                .Include(i => i.Tenant)
                .Include(i => i.Penalties)
                .ThenInclude(x => x.ShippingRequestTripFK)
                .ThenInclude(x => x.ShippingRequestFk)
                .Include(x => x.Penalties)
                .ThenInclude(x => x.ShippingRequestTripFK)
                .ThenInclude(x => x.AssignedTruckFk)
                .FirstOrDefaultAsync(i => i.Id == penaltyInvoiceId);
            if (invoice == null) throw new UserFriendlyException(L("TheInvoiceNotFound"));

            return invoice;
        }

        private async Task<DedicatedDynamicInvoice> GetDedicatedDynamicInvoiceInfo(long invoiceId)
        {
            DisableTenancyFilters();
            var invoice = await _dedicatedDynamicInvoiceRepository
                .GetAll()
                .Include(i => i.Tenant)
                .Include(i => i.DedicatedDynamicInvoiceItems)
                .ThenInclude(x => x.DedicatedShippingRequestTruck)
                .ThenInclude(x => x.ShippingRequest)
                .ThenInclude(x => x.OriginCityFk)
                .Include(i => i.DedicatedDynamicInvoiceItems)
                .ThenInclude(x => x.DedicatedShippingRequestTruck)
                .ThenInclude(x => x.Truck)
                .ThenInclude(x => x.TrucksTypeFk)
                .Include(i => i.DedicatedDynamicInvoiceItems)
                .ThenInclude(x => x.DedicatedShippingRequestTruck)
                .ThenInclude(x => x.ShippingRequest)
                .ThenInclude(x=>x.ShippingRequestDestinationCities)
                .ThenInclude(x=>x.CityFk)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
            if (invoice == null) throw new UserFriendlyException(L("TheInvoiceNotFound"));

            return invoice;
        }

        private async Task<DynamicInvoice> GetDynamicInvoiceInfo (long invoiceId)
        {
            DisableTenancyFilters();
            var invoice = await _dynamicInvoiceRepository
                .GetAll()
                .Include(i => i.CreditTenant)
                .Include(i => i.DebitTenant)
                .Include(i => i.Items)
                .ThenInclude(x => x.ShippingRequestTrip)
                .ThenInclude(x => x.ShippingRequestFk)
                .ThenInclude(x => x.OriginCityFk)
                .Include(i => i.Items)
                .ThenInclude(x => x.ShippingRequestTrip)
                .ThenInclude(x => x.ShippingRequestFk)
                .ThenInclude(x => x.DestinationCityFk)
                .Include(i => i.Items)
                .ThenInclude(x => x.ShippingRequestTrip)
                .ThenInclude(x => x.AssignedTruckFk)
                .ThenInclude(x => x.TrucksTypeFk)
                .Include(i => i.Items)
                .ThenInclude(x => x.OriginCity)
                .Include(i => i.Items)
                .ThenInclude(x => x.DestinationCity)
                .Include(i => i.Items)
                .ThenInclude(x => x.Truck).ThenInclude(x => x.TrucksTypeFk) // todo review this with Tasneem 
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
            if (invoice == null) throw new UserFriendlyException(L("TheInvoiceNotFound"));

            return invoice;
        }
        private List<InvoiceItemDto> GetInvoiceItems(Invoice invoice)
        {
            var TotalItem = invoice.Trips.Count +
                            invoice.Trips.Sum(v => v.ShippingRequestTripFK.ShippingRequestTripVases.Count);
            int Sequence = 1;
            List<InvoiceItemDto> Items = new List<InvoiceItemDto>();
            invoice.Trips.ToList().ForEach(trip =>
            {
                int VasCounter = 0;
                Items.Add(new InvoiceItemDto
                {
                    Sequence = $"{Sequence}/{TotalItem}",
                    SubTotalAmount =
                        AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier)
                            ? trip.ShippingRequestTripFK.SubTotalAmount.Value
                            : trip.ShippingRequestTripFK.SubTotalAmountWithCommission.Value,
                    VatAmount =
                        AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier)
                            ? trip.ShippingRequestTripFK.VatAmount.Value
                            : trip.ShippingRequestTripFK.VatAmountWithCommission.Value,
                    TotalAmount =
                        AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier)
                            ? trip.ShippingRequestTripFK.TotalAmount.Value
                            : trip.ShippingRequestTripFK.TotalAmountWithCommission.Value,
                    WayBillNumber = trip.ShippingRequestTripFK.WaybillNumber.ToString(),
                    TruckType =
                        ObjectMapper.Map<TrucksTypeDto>(trip.ShippingRequestTripFK.AssignedTruckFk.TrucksTypeFk)
                            .TranslatedDisplayName,
                    Source =
                        ObjectMapper.Map<CityDto>(trip.ShippingRequestTripFK.ShippingRequestFk.OriginCityFk)
                            ?.TranslatedDisplayName ??
                        trip.ShippingRequestTripFK.ShippingRequestFk.OriginCityFk.DisplayName,
                    //Destination =
                    //    ObjectMapper.Map<CityDto>(trip.ShippingRequestTripFK.ShippingRequestFk.DestinationCityFk)
                    //        ?.TranslatedDisplayName ??
                    //    trip.ShippingRequestTripFK.ShippingRequestFk.DestinationCityFk.DisplayName,
                    Destination = trip.ShippingRequestTripFK.DestinationFacilityFk.CityFk.DisplayName,
                    DateWork =
                        trip.ShippingRequestTripFK.ShippingRequestFk.EndTripDate.HasValue
                            ? trip.ShippingRequestTripFK.ShippingRequestFk.EndTripDate.Value.ToString(
                                "dd MMM, yyyy")
                            : "",
                    Remarks = trip.ShippingRequestTripFK.ShippingRequestFk.RouteTypeId ==
                              Shipping.ShippingRequests.ShippingRequestRouteType.MultipleDrops
                        ? L("TotalOfDrop", trip.ShippingRequestTripFK.ShippingRequestFk.NumberOfDrops)
                        : ""
                });
                Sequence++;
                if (trip.ShippingRequestTripFK.ShippingRequestTripVases != null &&
                    trip.ShippingRequestTripFK.ShippingRequestTripVases.Count > 1)
                {
                    VasCounter = 1;
                }

                foreach (var vas in trip.ShippingRequestTripFK.ShippingRequestTripVases)
                {
                    string waybillnumber;
                    if (VasCounter == 0)
                    {
                        waybillnumber = $"{trip.ShippingRequestTripFK.WaybillNumber.ToString()}VAS";
                    }
                    else
                    {
                        waybillnumber = $"{trip.ShippingRequestTripFK.WaybillNumber.ToString()}VAS{VasCounter}";
                        VasCounter++;
                    }

                    trip.ShippingRequestTripFK.WaybillNumber.ToString();

                    var item = new InvoiceItemDto
                    {
                        Sequence = $"{Sequence}/{TotalItem}",
                        SubTotalAmount =
                            AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier)
                                ? vas.SubTotalAmount.Value
                                : vas.SubTotalAmountWithCommission.Value,
                        VatAmount =
                            AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier)
                                ? vas.VatAmount.Value
                                : vas.VatAmountWithCommission.Value,
                        TotalAmount =
                            AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier)
                                ? vas.TotalAmount.Value
                                : vas.TotalAmountWithCommission.Value,
                        WayBillNumber = waybillnumber,
                        TruckType = L("InvoiceVasType", vas.ShippingRequestVasFk.VasFk.Key),
                        Source = "-",
                        Destination = "-",
                        DateWork = "-",
                        Remarks = vas.Quantity > 1 ? $"{vas.Quantity}" : ""
                    };
                    Items.Add(item);

                    Sequence++;
                }
            });
            return Items;
        }

        public async Task<bool> MakePaid(long invoiceId)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer);

            var Invoice = await GetInvoice(invoiceId);
            if (Invoice != null && !Invoice.IsPaid)
            {
                return await _commonManager.ExecuteMethodIfHostOrTenantUsers(async () =>
                {
                    if (!_invoiceManager.IsTenantCarrier(Invoice.TenantId))
                    {
                        if (await _balanceManager.CheckShipperCanPaidFromBalance(Invoice.TenantId, Invoice.TotalAmount))
                        {
                            await _balanceManager.AddBalanceToShipper(Invoice.TenantId, -Invoice.TotalAmount);

                            if ((InvoicePeriodType)Invoice.PeriodId != InvoicePeriodType.PayInAdvance)
                            {
                                await _balanceManager.AddCreditBalanceToShipper(Invoice.TenantId, Invoice.TotalAmount);
                            }
                        }
                        else
                            return false;
                    }
                    else
                    {
                        await _balanceManager.AddBalanceToCarrier(Invoice.TenantId, +Invoice.TotalAmount);
                    }

                    Invoice.IsPaid = true;
                    Invoice.PaymentDate = Clock.Now;
                    await _transactionManager.Create(new Transaction
                    {
                        Amount = Invoice.TotalAmount,
                        ChannelId = ChannelType.Invoices,
                        TenantId = Invoice.TenantId,
                        SourceId = Invoice.Id,
                    });

                    return true;
                });
            }

            return false;
        }

        //[AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_MakeUnPaid)]
        public async Task MakeUnPaid(long invoiceId)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer);

            var Invoice = await GetInvoice(invoiceId);
            if (Invoice != null && Invoice.IsPaid)
            {
                if (!_invoiceManager.IsTenantCarrier(Invoice.TenantId))
                {
                    await _balanceManager.AddBalanceToShipper(Invoice.TenantId, Invoice.TotalAmount);
                    if ((InvoicePeriodType)Invoice.PeriodId != InvoicePeriodType.PayInAdvance)
                    {
                        await _balanceManager.AddCreditBalanceToShipper(Invoice.TenantId, -Invoice.TotalAmount);
                    }
                }
                else
                {
                    await _balanceManager.AddBalanceToCarrier(Invoice.TenantId, -Invoice.TotalAmount);
                }

                await _transactionManager.Delete(Invoice.Id, ChannelType.Invoices);
                Invoice.IsPaid = false;
            }
        }
        public async Task OnDemand(int Id, List<SelectItemDto> waybills)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer);
            DisableTenancyFilters();
            var tenant = await TenantManager.GetByIdAsync(Id);

            // if (tenant == null || tenant.Name == AppConsts.ShipperEditionName) throw new UserFriendlyException(L("TheTenantSelectedIsNotShipper"));
            await _invoiceManager.GenertateInvoiceOnDeman(tenant, waybills.Select(x => x.Id).Select(int.Parse).ToList());
        }

        [AbpAuthorize(AppPermissions.Pages_Invoices_ConfirmInvoice)]
        public async Task ConfirmInvoice(long invoiceId)
        {
            await _invoiceManager.ConfirmInvoice(invoiceId);
        }
        public async Task DynamicInvoiceOnDemand(long dynamicInvoiceId)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer);
            DisableTenancyFilters();

            var dynamicInvoice = await _dynamicInvoiceRepository.GetAll()
                .Include(x => x.Items)
                .Include(x => x.CreditTenant)
                .Include(x => x.DebitTenant)
                .SingleAsync(x => x.Id == dynamicInvoiceId);

            if (dynamicInvoice.InvoiceId.HasValue || dynamicInvoice.SubmitInvoiceId.HasValue)
                throw new UserFriendlyException(L("ThisInvoiceAlreadyGenerated"));
            
            var tenant = default(Tenant);
            if (dynamicInvoice.CreditTenantId != null)
            {
                //generate invoice
                tenant = dynamicInvoice.CreditTenant;

                await _invoiceManager.GenerateDynamicInvoice(tenant, dynamicInvoice);
            }
            else if (dynamicInvoice.DebitTenantId != null)
            {
                //generate submit invoice
                tenant = dynamicInvoice.DebitTenant;
                await _invoiceManager.GenerateSubmitDynamicInvoice(tenant, dynamicInvoice);
            }

        }

        public async Task GeneratePenaltyInvoiceOnDemand(int tenantId, int? penaltyId)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer);
            DisableTenancyFilters();
            var tenant = await TenantManager.GetByIdAsync(tenantId);

            await _invoiceManager.GeneratePenaltyInvoiceOnDemand(tenant, penaltyId);
        }

       
        public async Task<List<SelectItemDto>> GetUnInvoicedWaybillsByTenant(int tenantId)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            return await _shippingRequestTripRepository.GetAll()
                .Where(
                x => (x.ShippingRequestFk.TenantId == tenantId && !x.IsShipperHaveInvoice) &&
                (x.Status == ShippingRequestTripStatus.Delivered ||
                    x.InvoiceStatus == InvoiceTripStatus.CanBeInvoiced)
                )
                .Select(x => new SelectItemDto { DisplayName = x.WaybillNumber.ToString(), Id = x.Id.ToString() })
                .ToListAsync();

        }

        [AbpAllowAnonymous]
        public async Task<InvoiceOutSideDto> GetInvoiceOutSide(EntityDto<long> input)
        {
            DisableTenancyFilters();
            var invoiceDto = await _invoiceRepository.GetAll()
                .Where(x => x.Id == input.Id)
                .ProjectTo<InvoiceOutSideDto>(AutoMapperConfigurationProvider)
                .FirstOrDefaultAsync();

            var documentVat = await _documentFileRepository.FirstOrDefaultAsync(x => x.TenantId == invoiceDto.TenantId && x.DocumentTypeId == 15);
            if (documentVat != null) invoiceDto.VATNumber = documentVat.Number;
            return invoiceDto;
        }

        public async Task CorrectShipperInvoice(int invoiceId)
        {
            await _invoiceManager.CorrectShipperInvoice(invoiceId);
        }

        #region Reports

        public IEnumerable<InvoiceInfoDto> GetInvoiceReportInfo(long invoiceId)
        {
            var bankNameArabic = SettingManager.GetSettingValue(AppSettings.Invoice.BankNameArabic);
            var bankNameEnglish = SettingManager.GetSettingValue(AppSettings.Invoice.BankNameEnglish);

            DisableTenancyFilters();
            AsyncHelper.RunSync(() => DisableInvoiceDraftedFilter());
            var invoice = _invoiceRepository
                .GetAll()
                .Include(i => i.InvoicePeriodsFK)
                .Include(i => i.Tenant)
                .FirstOrDefault(i => i.Id == invoiceId);

            if (invoice == null) throw new UserFriendlyException(L("TheInvoiceNotFound"));

            string financialEmail = invoice.Tenant?.FinancialEmail;
            string financialName = invoice.Tenant?.FinancialName;
            string financialPhone = invoice.Tenant?.FinancialPhone;
            
            var invoiceDto = ObjectMapper.Map<InvoiceInfoDto>(invoice);

            var admin = AsyncHelper.RunSync(() => _userManager.GetAdminByTenantIdAsync(invoice.TenantId));
            invoiceDto.TaxVat = _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);
            invoiceDto.Phone = financialPhone ?? admin.PhoneNumber;
            invoiceDto.Email = financialEmail ?? admin.EmailAddress;
            invoiceDto.Attn = financialName ?? admin.FullName;
            invoiceDto.BankNameArabic = bankNameArabic;
            invoiceDto.BankNameEnglish = bankNameEnglish;
            var document = AsyncHelper.RunSync(() =>
                _documentFileRepository.FirstOrDefaultAsync(x =>
                    x.TenantId == invoice.TenantId && x.DocumentTypeId == 14));
            if (document != null) invoiceDto.CR = document.Number;
            var documentVat = AsyncHelper.RunSync(() =>
                _documentFileRepository.FirstOrDefaultAsync(x =>
                    x.TenantId == invoice.TenantId && x.DocumentTypeId == 15));
            if (document != null) invoiceDto.TenantVatNumber = documentVat.Number;
            var link = $"{_webUrlService.WebSiteRootAddressFormat }account/outsideInvoice?id={invoiceId}";
            invoiceDto.QRCode = _pdfExporterBase.GenerateQrCode(link);
            invoiceDto.Note = invoice.Note;
            return new List<InvoiceInfoDto>() { invoiceDto };
        }

        public IEnumerable<InvoiceItemDto> GetInvoiceShippingRequestsReportInfo(long invoiceId)
        {
            DisableTenancyFilters();
            var invoice = AsyncHelper.RunSync(() => GetInvoiceInfo(invoiceId));

            if (invoice == null) throw new UserFriendlyException(L("TheInvoiceNotFound"));
            var TotalItem = invoice.Trips.Count +
                            invoice.Trips.SelectMany(v => v.ShippingRequestTripFK.ShippingRequestTripVases).Count();
            int Sequence = 1;
            List<InvoiceItemDto> Items = new List<InvoiceItemDto>();
            foreach (var trip in invoice.Trips.ToList())
            {
                int VasCounter = 0;
                Items.Add(new InvoiceItemDto
                {
                    Sequence = $"{Sequence}/{TotalItem}",
                    SubTotalAmount = trip.ShippingRequestTripFK.SubTotalAmountWithCommission.Value,
                    VatAmount = trip.ShippingRequestTripFK.VatAmountWithCommission.Value,
                    TotalAmount = trip.ShippingRequestTripFK.TotalAmountWithCommission.Value,
                    WayBillNumber = trip.ShippingRequestTripFK.WaybillNumber.ToString(),
                    TruckType = ObjectMapper.Map<TrucksTypeDto>(trip.ShippingRequestTripFK.AssignedTruckFk.TrucksTypeFk).TranslatedDisplayName,
                    Source = ObjectMapper.Map<CityDto>(trip.ShippingRequestTripFK.ShippingRequestFk.OriginCityFk)?.TranslatedDisplayName ?? trip.ShippingRequestTripFK.ShippingRequestFk.OriginCityFk.DisplayName,
                    Destination = trip.ShippingRequestTripFK.DestinationFacilityFk.CityFk.DisplayName,
                    DateWork = trip.ShippingRequestTripFK.EndTripDate.HasValue ? trip.ShippingRequestTripFK.EndTripDate.Value.ToString("dd/MM/yyyy") : trip.InvoiceFK.CreationTime.ToString("dd/MM/yyyy"),
                    Remarks = trip.ShippingRequestTripFK.ShippingRequestFk.RouteTypeId == Shipping.ShippingRequests.ShippingRequestRouteType.MultipleDrops ?
                        L("TotalOfDrop", trip.ShippingRequestTripFK.ShippingRequestFk.NumberOfDrops) : "",
                    ContainerNumber = trip.ShippingRequestTripFK.CanBePrinted ? trip.ShippingRequestTripFK.ContainerNumber ?? "-" : "-",
                    RoundTrip = trip.ShippingRequestTripFK.CanBePrinted ? trip.ShippingRequestTripFK.RoundTrip ?? "-" : "-",
                });
                Sequence++;
                if (trip.ShippingRequestTripFK.ShippingRequestTripVases != null &&
                    trip.ShippingRequestTripFK.ShippingRequestTripVases.Count > 1)
                {
                    VasCounter = 1;
                }

                if (trip.ShippingRequestTripFK.ShippingRequestFk.IsSaas())
                {
                    continue;

                }
                foreach (var vas in trip.ShippingRequestTripFK.ShippingRequestTripVases)
                {
                    string waybillnumber;
                    if (VasCounter == 0)
                    {
                        waybillnumber = $"{trip.ShippingRequestTripFK.WaybillNumber.ToString()}VAS";
                    }
                    else
                    {
                        waybillnumber = $"{trip.ShippingRequestTripFK.WaybillNumber.ToString()}VAS{VasCounter}";
                        VasCounter++;
                    }

                    trip.ShippingRequestTripFK.WaybillNumber.ToString();

                    var item = new InvoiceItemDto
                    {
                        Sequence = $"{Sequence}/{TotalItem}",
                        SubTotalAmount =
                            AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier)
                                ? vas.SubTotalAmount.Value
                                : vas.SubTotalAmountWithCommission.Value,
                        VatAmount =
                            AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier)
                                ? vas.VatAmount.Value
                                : vas.VatAmountWithCommission.Value,
                        TotalAmount =
                            AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier)
                                ? vas.TotalAmount.Value
                                : vas.TotalAmountWithCommission.Value,
                        WayBillNumber = waybillnumber,
                        TruckType = L("InvoiceVasType", vas.ShippingRequestVasFk.VasFk.Key),
                        Source = "-",
                        Destination = "-",
                        DateWork = "-",
                        Remarks = vas.Quantity > 1 ? $"{vas.Quantity}" : ""
                    };
                    Items.Add(item);

                    Sequence++;
                }

            }


            //var invoiceDto = ObjectMapper.Map<InvoiceInfoDto>(invoice);
            //var Admin = AsyncHelper.RunSync(() => _userManager.GetAdminByTenantIdAsync(invoice.TenantId);
            //invoiceDto.Items = Items;
            //invoiceDto.Phone = Admin.PhoneNumber;
            //invoiceDto.Email = Admin.EmailAddress;
            //DisableTenancyFilters();
            //var documnet = AsyncHelper.RunSync(() => _documentFileRepository.FirstOrDefaultAsync(x => x.TenantId == invoice.TenantId && x.DocumentTypeId == 14);
            //if (documnet != null) invoiceDto.CR = documnet.Number;
            return Items;
        }
        public IEnumerable<PeanltyInvoiceItemDto> GetInvoicePenaltiseInvoiceReportInfo(long penaltynvoiceId)
        {
            var pnealtyInvoice = AsyncHelper.RunSync(() => GetPenaltyInvoiceInfo(penaltynvoiceId));

            if (pnealtyInvoice == null) throw new UserFriendlyException(L("TheInvoiceNotFound"));
            var TotalItem = pnealtyInvoice.Penalties.Count();
            int Sequence = 1;
            List<PeanltyInvoiceItemDto> Items = new List<PeanltyInvoiceItemDto>();
            pnealtyInvoice.Penalties.ToList().ForEach(penalty =>
            {
                Items.Add(new PeanltyInvoiceItemDto
                {
                    Sequence = $"{Sequence}/{TotalItem}",
                    PenaltyName = penalty.PenaltyName,
                    VatAmount = penalty.VatPostCommestion,
                    TotalAmount = penalty.TotalAmount,
                    Date = penalty.CreationTime.ToString("dd/MM/yyyy"),
                    ContainerNumber = penalty.ShippingRequestTripFK != null ? penalty.ShippingRequestTripFK.AssignedTruckFk?.PlateNumber : "-",
                    WayBillNumber = penalty.ShippingRequestTripFK != null ? penalty.ShippingRequestTripFK.WaybillNumber.ToString() : "-",
                    ItmePrice = penalty.AmountPostCommestion,
                    Remarks = penalty.ShippingRequestTripFK != null ? penalty.ShippingRequestTripFK.ShippingRequestFk.RouteTypeId ==
                              Shipping.ShippingRequests.ShippingRequestRouteType.MultipleDrops
                        ? L("TotalOfDrop", penalty.ShippingRequestTripFK.ShippingRequestFk.NumberOfDrops)
                        : "" : ""
                });
                Sequence++;
            });
            return Items;
        }

        public IEnumerable<InvoiceItemDto> GetDynamicInvoiceItemsReportInfo(long invoiceId)
        {
            var dynamicInvoice = AsyncHelper.RunSync(() => GetDynamicInvoiceInfo(invoiceId));

            if (dynamicInvoice == null) throw new UserFriendlyException(L("TheInvoiceNotFound"));
            var TotalItem = dynamicInvoice.Items.Count();
            int Sequence = 1;
            List<InvoiceItemDto> Items = new List<InvoiceItemDto>();
            List<DynamicInvoiceItem> dynamicInvoiceItems = dynamicInvoice.Items.ToList();

            foreach (var item in dynamicInvoiceItems)
            {
                InvoiceItemDto invoiceItemDto = new InvoiceItemDto
                {
                    Sequence = $"{Sequence}/{TotalItem}",
                    SubTotalAmount = item.Price,
                    VatAmount = item.VatAmount,
                    TotalAmount = item.TotalAmount,
                    RoundTrip = item.Description,

                };

                //WayBillNumber
                if (item.ShippingRequestTrip != null)
                {
                    invoiceItemDto.WayBillNumber = item.ShippingRequestTrip.WaybillNumber.ToString();
                }
                else
                {
                    invoiceItemDto.WayBillNumber = "";
                }

                //TruckType
                if (item.ShippingRequestTrip != null)
                {
                    invoiceItemDto.TruckType = ObjectMapper.Map<TrucksTypeDto>(item.ShippingRequestTrip.AssignedTruckFk.TrucksTypeFk).TranslatedDisplayName;
                }
                else
                {
                    if (item.Truck != null)
                    {
                        invoiceItemDto.TruckType = ObjectMapper.Map<TrucksTypeDto>(item.Truck.TrucksTypeFk).TranslatedDisplayName;
                    }
                }

                //Source
                if (item.ShippingRequestTrip != null)
                {
                    var cityDto = ObjectMapper.Map<CityDto>(item.ShippingRequestTrip.ShippingRequestFk.OriginCityFk);

                    if (cityDto != null)
                    {
                        invoiceItemDto.Source = cityDto.NormalizedDisplayName;

                    }

                }
                else
                {
                    if (item.OriginCity != null)
                    {
                        CityDto cityDto = ObjectMapper.Map<CityDto>(item.OriginCity);
                        invoiceItemDto.Source = cityDto.NormalizedDisplayName;
                    }
                }

                //Destination
                if (item.ShippingRequestTrip != null)
                {
                    CityDto cityDto = ObjectMapper.Map<CityDto>(item.ShippingRequestTrip.ShippingRequestFk.DestinationCityFk);
                    invoiceItemDto.Destination = cityDto.NormalizedDisplayName;

                }
                else
                {
                    if (item.DestinationCity != null)
                    {
                        CityDto cityDto = ObjectMapper.Map<CityDto>(item.DestinationCity);
                        invoiceItemDto.Destination = cityDto.NormalizedDisplayName;
                    }
                }

                //DateWork
                if (item.ShippingRequestTrip != null)
                {
                    if (item.ShippingRequestTrip.EndTripDate.HasValue)
                    {
                        invoiceItemDto.DateWork = item.ShippingRequestTrip.EndTripDate.Value.Date.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        if (item.WorkDate != null)
                        {
                            invoiceItemDto.DateWork = item.WorkDate.Value.Date.ToString("dd/MM/yyyy");
                        }
                    }
                }
                else
                {
                    if (item.WorkDate != null)
                    {
                        invoiceItemDto.DateWork = item.WorkDate.Value.ToString("dd/MM/yyyy");
                    }
                }


                //Remarks
                if (item.ShippingRequestTrip != null)
                {
                    int numberOfDrops = item.ShippingRequestTrip.ShippingRequestFk.NumberOfDrops;
                    if (numberOfDrops > 1)
                    {
                        invoiceItemDto.Remarks = L("TotalOfDrop", numberOfDrops);
                    }
                    else
                    {
                        invoiceItemDto.Remarks = "";
                    }
                    
                }
                else
                {
                    if (item.Quantity != null)
                    {
                        invoiceItemDto.Remarks = item.Quantity.Value.ToString();
                    }
                }

                //ContainerNumber
                if (item.ShippingRequestTrip != null)
                {
                    invoiceItemDto.ContainerNumber = item.ShippingRequestTrip.ContainerNumber;
                }
                else
                {
                    invoiceItemDto.ContainerNumber = item.ContainerNumber;
                }

                // PlateNumber
                if (item.ShippingRequestTrip != null)
                {
                    invoiceItemDto.PlateNumber = item.ShippingRequestTrip.AssignedTruckFk.PlateNumber;
                }



                Items.Add(invoiceItemDto);

                Sequence++;
            }

            return Items;
        }


        public IEnumerable<DedicatedDynamicInvoiceItemDto> GetDedicatedDynamicInvoiceItemsReportInfo(long invoiceId)
        {
            var dedicatedInvoice = AsyncHelper.RunSync(() => GetDedicatedDynamicInvoiceInfo(invoiceId));

            if (dedicatedInvoice == null) throw new UserFriendlyException(L("TheInvoiceNotFound"));
            var TotalItem = dedicatedInvoice.DedicatedDynamicInvoiceItems.Count();
            int Sequence = 1;
            List<DedicatedDynamicInvoiceItemDto> Items = new List<DedicatedDynamicInvoiceItemDto>();
            List<DedicatedDynamicInvoiceItem> dedicatednvoiceItems = dedicatedInvoice.DedicatedDynamicInvoiceItems.ToList();

            foreach (var item in dedicatednvoiceItems)
            {
                DedicatedDynamicInvoiceItemDto invoiceItemDto = new DedicatedDynamicInvoiceItemDto
                {
                    Sequence = $"{Sequence}/{TotalItem}",
                    SubTotalAmount = item.ItemSubTotalAmount,
                    VatAmount = item.VatAmount,
                    TotalAmount = item.ItemTotalAmount,
                    Remarks = item.WorkingDayType == DedicatedDynamicInvocies.WorkingDayType.OverTime ? "Over Time" : item.Remarks,
                    Duration = $"{item.NumberOfDays} {"Days"}",
                    PricePerDay = item.PricePerDay,
                    TruckType = item.DedicatedShippingRequestTruck.ReplacementFlag == Shipping.Dedicated.ReplacementFlag.Replaced 
                    ?$"{item.DedicatedShippingRequestTruck.Truck.TrucksTypeFk.DisplayName}{"<br/>"}{"Replacement"}{item.DedicatedShippingRequestTruck.OriginalTruck.Truck.PlateNumber}" 
                    :item.DedicatedShippingRequestTruck.Truck.TrucksTypeFk.DisplayName,
                    TruckPlateNumber = item.DedicatedShippingRequestTruck.Truck.PlateNumber,
                    TaxVat = item.TaxVat,
                    Date = item.CreationTime.ToString("dd/MM/yyyy"),
                    From = item.DedicatedShippingRequestTruck.ShippingRequest.ShippingRequestDestinationCities.First().CityFk.DisplayName,
                    To = item.DedicatedShippingRequestTruck.ShippingRequest.ShippingRequestDestinationCities.Count() > 0 ? "Multiple dedtinations"
                    : item.DedicatedShippingRequestTruck.ShippingRequest.ShippingRequestDestinationCities.First().CityFk.DisplayName,
                };
                Items.Add(invoiceItemDto);
                Sequence++;
            }
            return Items;

        }

        // Actors invoice
        public async Task<IEnumerable<ActorInvoiceInfoDto>> GetActorShipperInvoiceReportInfo(long actorInvoiceId)
        {

            DisableTenancyFilters();
            var invoice = _actorInvoiceRepository
                .GetAll()
                .Include(i => i.ShipperActorFk)
                .Include(i => i.Tenant)
                .FirstOrDefault(i => i.Id == actorInvoiceId);



            if (invoice == null) throw new UserFriendlyException(L("TheInvoiceNotFound"));

            // mapping
            var actorInvoiceDto = ObjectMapper.Map<ActorInvoiceInfoDto>(invoice);

            //logo
            if (invoice.Tenant.HasLogo())
            {
                var logo = await _binaryObjectManager.GetOrNullAsync(invoice.Tenant.LogoId.Value);
                byte[] logoBytes = logo.Bytes;
                actorInvoiceDto.Logo = logoBytes;
            }



            //broker name 

            actorInvoiceDto.BrokerName = invoice.Tenant.Name;

            //attn info


            actorInvoiceDto.Phone = invoice.ShipperActorFk.MobileNumber;
            actorInvoiceDto.Email = invoice.ShipperActorFk.Email;
            actorInvoiceDto.Attn = invoice.ShipperActorFk.CompanyName;

            //Broker invoice settings info 
            actorInvoiceDto.BrokerBankNameArabic = await SettingManager.GetSettingValueForTenantAsync(AppSettings.Invoice.BrokerBankNameArabic, invoice.TenantId);
            actorInvoiceDto.BrokerBankNameEnglish = await SettingManager.GetSettingValueForTenantAsync(AppSettings.Invoice.BrokerBankNameEnglish, invoice.TenantId);
            actorInvoiceDto.BrokerBankAccountNumber = await SettingManager.GetSettingValueForTenantAsync(AppSettings.Invoice.BrokerBankAccountNumber, invoice.TenantId);
            actorInvoiceDto.BrokerIban = await SettingManager.GetSettingValueForTenantAsync(AppSettings.Invoice.BrokerIban, invoice.TenantId);
            actorInvoiceDto.BrokerEmailAddress = await SettingManager.GetSettingValueForTenantAsync(AppSettings.Invoice.BrokerEmailAddress, invoice.TenantId);
            actorInvoiceDto.BrokerWebSite = await SettingManager.GetSettingValueForTenantAsync(AppSettings.Invoice.BrokerWebSite, invoice.TenantId);
            actorInvoiceDto.BrokerAddress = await SettingManager.GetSettingValueForTenantAsync(AppSettings.Invoice.BrokerAddress, invoice.TenantId);
            actorInvoiceDto.BrokerMobile = await SettingManager.GetSettingValueForTenantAsync(AppSettings.Invoice.BrokerMobile, invoice.TenantId);

            // broker Cr & Vat 
            var cr = AsyncHelper.RunSync(() =>
             _documentFileRepository.FirstOrDefaultAsync(x =>
                 x.TenantId == invoice.TenantId && x.DocumentTypeFk.Flag == DocumentTypeFlagEnum.Cr));
            if (cr != null) actorInvoiceDto.BrokerCr = cr.Number;

            var vat = AsyncHelper.RunSync(() =>
                _documentFileRepository.FirstOrDefaultAsync(x =>
                  x.TenantId == invoice.TenantId && x.DocumentTypeFk.Flag == DocumentTypeFlagEnum.Vat));
            if (vat != null) actorInvoiceDto.BrokerVat = vat.Number;





            //Actor Cr & Vat
            var document = AsyncHelper.RunSync(() =>
                _documentFileRepository.FirstOrDefaultAsync(x =>
                    x.ActorId == invoice.ShipperActorId && x.DocumentTypeFk.Flag == DocumentTypeFlagEnum.Cr));
            if (document != null) actorInvoiceDto.CR = document.Number;

            var documentVat = AsyncHelper.RunSync(() =>
                _documentFileRepository.FirstOrDefaultAsync(x =>
                  x.ActorId == invoice.ShipperActorId && x.DocumentTypeFk.Flag == DocumentTypeFlagEnum.Vat));
            if (document != null) actorInvoiceDto.TenantVatNumber = documentVat.Number;

            return new List<ActorInvoiceInfoDto>() { actorInvoiceDto };
        }


        public IEnumerable<InvoiceItemDto> GetActorInvoiceShippingRequestsReportInfo(long actorInvoiceId)
        {
            DisableTenancyFilters();
            var actorInvoice = AsyncHelper.RunSync(() => GetActorInvoiceInfo(actorInvoiceId));

            if (actorInvoice == null) throw new UserFriendlyException(L("TheInvoiceNotFound"));
            var totalItem = actorInvoice.Trips.Count + actorInvoice.Trips.SelectMany(v => v.ShippingRequestTripVases).Count();
            int sequence = 1;
            List<InvoiceItemDto> items = new List<InvoiceItemDto>();
            foreach (var trip in actorInvoice.Trips.ToList())
            {
                int vasCounter = 0;
                items.Add(new InvoiceItemDto
                {
                    Sequence = $"{sequence}/{totalItem}",
                    SubTotalAmount = trip.ShippingRequestFk.ActorShipperPrice.SubTotalAmountWithCommission.Value,
                    VatAmount = trip.ShippingRequestFk.ActorShipperPrice.VatAmountWithCommission.Value,
                    TotalAmount = trip.ShippingRequestFk.ActorShipperPrice.TotalAmountWithCommission.Value,
                    WayBillNumber = trip.WaybillNumber.ToString(),
                    TruckType = trip.AssignedTruckFk != null ? ObjectMapper.Map<TrucksTypeDto>(trip.AssignedTruckFk.TrucksTypeFk).TranslatedDisplayName : "",
                    Source = ObjectMapper.Map<CityDto>(trip.ShippingRequestFk.OriginCityFk)?.TranslatedDisplayName ?? trip.ShippingRequestFk.OriginCityFk.DisplayName,
                    Destination = trip.ShippingRequestFk.ShippingRequestDestinationCities.First().CityFk.DisplayName,
                    DateWork = trip.EndTripDate.HasValue ? trip.EndTripDate.Value.ToString("dd/MM/yyyy") : trip.ActorInvoiceFk.CreationTime.ToString("dd/MM/yyyy"),
                    Remarks = trip.ShippingRequestFk.RouteTypeId == Shipping.ShippingRequests.ShippingRequestRouteType.MultipleDrops ?
                       L("TotalOfDrop", trip.ShippingRequestFk.NumberOfDrops) : "",
                    ContainerNumber = trip.CanBePrinted ? trip.ContainerNumber ?? "-" : "-",
                    RoundTrip = trip.CanBePrinted ? trip.RoundTrip ?? "-" : "-",
                });
                sequence++;
                if (trip.ShippingRequestTripVases != null &&
                    trip.ShippingRequestTripVases.Count > 1)
                {
                    vasCounter = 1;
                }


                foreach (var vas in trip.ShippingRequestTripVases)
                {
                    string waybillNumber;
                    if (vasCounter == 0)
                    {
                        waybillNumber = $"{trip.WaybillNumber.ToString()}VAS";
                    }
                    else
                    {
                        waybillNumber = $"{trip.WaybillNumber.ToString()}VAS{vasCounter}";
                        vasCounter++;
                    }

                    trip.WaybillNumber.ToString();

                    var item = new InvoiceItemDto
                    {
                        Sequence = $"{sequence}/{totalItem}",
                        SubTotalAmount = vas.ShippingRequestVasFk.ActorShipperPrice.SubTotalAmountWithCommission.Value,
                        VatAmount = vas.ShippingRequestVasFk.ActorShipperPrice.VatAmountWithCommission.Value,
                        TotalAmount = vas.ShippingRequestVasFk.ActorShipperPrice.TotalAmountWithCommission.Value,
                        WayBillNumber = waybillNumber,
                        TruckType = L("InvoiceVasType", vas.ShippingRequestVasFk.VasFk.Key),
                        Source = "-",
                        Destination = "-",
                        DateWork = "-",
                        Remarks = vas.Quantity > 1 ? $"{vas.Quantity}" : ""
                    };
                    items.Add(item);

                    sequence++;
                }

            }

            return items;
        }



        //public IEnumerable<GetInvoiceReportInfoOutput> GetInvoiceReportInfo(long invoiceId)
        //{
        //    //for host user
        //    if(AbpSession.TenantId==null && AbpSession.UserId != null)
        //    {
        //        DisableTenancyFilters();
        //    }
        //    var invoice=_invoiceRepository.GetAll()
        //        .Include(e=>e.Tenant)
        //        .Where(e => e.Id == invoiceId);
        //    var query = invoice.Select(x => new
        //    {
        //        InvoiceNo=x.Id,
        //        InvoiceDate=x.CreationTime,
        //        DueDate=x.DueDate,
        //        Attn="",
        //        Phone="",
        //        Fax="",
        //        Email="",
        //        ContractNo="",
        //        BillTo=x.Tenant.companyName,
        //        CR="",
        //        Address=x.Tenant.Address,
        //        ProjectName="",
        //        InvoiceSubTotal=x.SubTotalAmount,
        //        VatAmount=x.VatAmount,
        //        DueAmount=x.TotalAmount
        //    });
        //    var output = query.ToList().Select(x=>new GetInvoiceReportInfoOutput
        //    {
        //        VatAmount = x.VatAmount,
        //        Address = x.Address,
        //        DueDate = x.DueDate,
        //        Attn = x.Attn,
        //        BillTo = x.BillTo,
        //        CR = x.CR,
        //        ContractNo = x.ContractNo,
        //        DueAmount = x.DueAmount,
        //        Email = x.Email,
        //        Fax = x.Fax,
        //        InvoiceDate = x.InvoiceDate,
        //        InvoiceNo = x.InvoiceNo,
        //        InvoiceSubTotal = x.InvoiceSubTotal,
        //        Phone = x.Phone,
        //        ProjectName = x.ProjectName
        //    });
        //    return output;
        //}

        //public IEnumerable<GetInvoiceShippingRequestsReportInfoOutput> GetInvoiceShippingRequestsReportInfo(long invoiceId)
        //{
        //    var requests = _invoiceShippingRequestRepository.GetAll()
        //        .Include(x => x.ShippingRequests)
        //        .ThenInclude(x => x.DestinationCityFk)
        //        .Include(x => x.ShippingRequests)
        //        .ThenInclude(x => x.OriginCityFk)
        //        .Include(x => x.ShippingRequests.TrucksTypeFk)
        //        .Include(x => x.ShippingRequests.ShippingRequestTrips)
        //        .Where(e => e.InvoiceId == invoiceId)
        //        .ToList();

        //    var vasesList = _shippingRequestVasesRepository.GetAll()
        //        .Where(x => requests.Select
        //                (e => e.ShippingRequests.Id).Contains(x.ShippingRequestId))
        //        .ToList();

        //    var list = requests.GroupJoin(vasesList,
        //        vas => vas.RequestId,
        //        req => req.ShippingRequestId,
        //        (req, VasGroup) => new
        //        { Vases = VasGroup, req = req.ShippingRequests });

        //    var finalList = new List<GetInvoiceShippingRequestsReportInfoOutput>();

        //    foreach (var request in list)
        //    {
        //        if (request.req.StartTripDate != null)
        //        {
        //            finalList.Add(new GetInvoiceShippingRequestsReportInfoOutput
        //            {
        //                Price = request.req.Price.ToString(),
        //                Date = request.req.StartTripDate.Value,
        //                DestinationCityName = request.req.DestinationCityFk.DisplayName,
        //                Notes = request.req.NumberOfDrops > 1
        //                    ? "Total of drops" + request.req.NumberOfDrops
        //                    : "",
        //                OriginCityName = request.req.OriginCityFk.DisplayName,
        //                TruckType =ObjectMapper.Map<TrucksTypeDto>(request.req.TrucksTypeFk).TranslatedDisplayName,// request.req.TrucksTypeFk.DisplayName,
        //                WaybillNo = request.req.ShippingRequestTrips.FirstOrDefault()?.Id.ToString()
        //            });
        //        }

        //        int vasIndex = 0;
        //        foreach (var vas in request.Vases)
        //        {
        //            vasIndex = vasIndex + 1;
        //            finalList.Add(new GetInvoiceShippingRequestsReportInfoOutput
        //            {
        //                Price = request.req.Price.ToString(),
        //                Date = request.req.StartTripDate.Value,
        //                DestinationCityName = "-",
        //                Notes = "Count:" + vas.RequestMaxCount + " Amount:" + vas.RequestMaxAmount,
        //                OriginCityName = "-",
        //                TruckType = ObjectMapper.Map<TrucksTypeDto>(request.req.TrucksTypeFk).TranslatedDisplayName, //request.req.TrucksTypeFk.DisplayName,
        //                WaybillNo = request.req.ShippingRequestTrips.FirstOrDefault()?.Id.ToString() + "VAS" + vasIndex
        //            });
        //        }
        //    }
        //    return finalList;
        //}

        public async Task<FileDto> Exports(InvoiceFilterInput input)
        {
            string[] HeaderText;
            Func<InvoiceListDto, object>[] propertySelectors;
            if (AbpSession.TenantId == null)
            {
                HeaderText = new string[]
                {
                    "InvoiceNumber", "CompanyName", "Interval", "AccountInvoiceType", "TotalAmount", "DueDate",
                    "CreationTime", "Status"
                };
                propertySelectors = new Func<InvoiceListDto, object>[]
                {
                    _ => _.InvoiceNumber, _ => _.TenantName, _ => _.Period, _ => _.AccountTypeTitle,
                    _ => _.TotalAmount, _ => _.DueDate.ToShortDateString(), _ => _.CreationTime.ToShortDateString(),
                    _ => _.IsPaid ? "Paid" : "UnPaid"
                };
            }
            else
            {
                HeaderText = new string[]
                {
                    "InvoiceNumber", "Interval", "TotalAmount", "DueDate", "CreationTime", "Status"
                };
                propertySelectors = new Func<InvoiceListDto, object>[]
                {
                    _ => _.InvoiceNumber, _ => _.Period, _ => _.TotalAmount, _ => _.DueDate.ToShortDateString(),
                    _ => _.CreationTime.ToShortDateString(), _ => _.IsPaid ? "Paid" : "UnPaid"
                };
            }

            return await _commonManager.ExecuteMethodIfHostOrTenantUsers(async () =>
                {
                    var InvoiceListDto = ObjectMapper.Map<List<InvoiceListDto>>(await GetInvoices(input));
                    return _excelExporterManager.ExportToFile(InvoiceListDto, "Invoices", HeaderText,
                        propertySelectors);
                }
            );
        }
        public async Task<FileDto> ExportItems(long id)
        {
            var invoice = await GetInvoiceInfo(id);
            List<InvoiceItemDto> Items = GetInvoiceItems(invoice);

            var HeaderText = new string[]
            {
                "Sequence", "Date", "WaybillNumber", "CityOrigin", "DestinationDelivery", "TruckType", "Price",
                "Vat", "Total", "Quantity"
            };
            var propertySelectors = new Func<InvoiceItemDto, object>[]
            {
                _ => _.Sequence, _ => _.DateWork, _ => _.WayBillNumber, _ => _.Source, _ => _.Destination,
                _ => _.TruckType, _ => _.SubTotalAmount, _ => _.VatAmount, _ => _.TotalAmount, _ => _.Remarks
            };

            return _excelExporterInvoiceItemManager.ExportToFile(Items, "Invoice", HeaderText, propertySelectors);
        }

        #endregion

        #region Helper



        private async Task<PagedResultDto<InvoiceListDto>> GetInvoicesWithPaging(InvoiceFilterInput input)
        {
            var query = await GetInvoices(input);
            var pagedInvoices = query
                .OrderBy(input.Sorting ?? "IsPaid asc")
                .PageBy(input);

            var totalCount = await query.CountAsync();

            return new PagedResultDto<InvoiceListDto>(
                totalCount,
                ObjectMapper.Map<List<InvoiceListDto>>(pagedInvoices)
            );
        }

        private async Task<IQueryable<Invoice>> GetInvoices(InvoiceFilterInput input)
        {
            return
                _invoiceRepository
                    .GetAll()
                    .Include(i => i.InvoicePeriodsFK)
                    .Include(i => i.Tenant)
                    .ThenInclude(t => t.Edition)
                    .WhereIf(AbpSession.TenantId.HasValue && !await IsEnabledAsync(AppFeatures.TachyonDealer),
                        e => e.TenantId == AbpSession.TenantId.Value)
                    .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer),
                        e => true)
                    .WhereIf(input.AccountType.HasValue, e => e.AccountType == input.AccountType)
                    .WhereIf(input.IsPaid.HasValue, e => e.IsPaid == input.IsPaid)
                    .WhereIf(input.TenantId.HasValue, i => i.TenantId == input.TenantId)
                    .WhereIf(input.PeriodId.HasValue, i => i.PeriodId == input.PeriodId)
                    .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue,
                        i => i.CreationTime >= input.FromDate && i.CreationTime < input.ToDate)
                    .WhereIf(input.DueFromDate.HasValue && input.DueToDate.HasValue,
                        i => i.DueDate >= input.DueFromDate && i.DueDate < input.DueToDate)
                    .AsNoTracking();
        }

        private async Task<Invoice> GetInvoice(long invoiceId)
        {
            DisableTenancyFilters();
            return await _invoiceRepository
                .GetAll()
                .WhereIf(AbpSession.TenantId.HasValue && !await IsEnabledAsync(AppFeatures.TachyonDealer),
                    e => e.TenantId == AbpSession.TenantId.Value)
                .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), e => true)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);
        }

        #endregion
    }
}