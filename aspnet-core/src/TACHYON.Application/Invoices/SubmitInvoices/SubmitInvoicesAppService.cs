using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Timing;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Cities.Dtos;
using TACHYON.Common;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Dto;
using TACHYON.DynamicInvoices;
using TACHYON.Exporting;
using TACHYON.Features;
using TACHYON.Invoices.Balances;
using TACHYON.Invoices.Dto;
using TACHYON.Invoices.Groups.Dto;
using TACHYON.Invoices.PaymentMethods;
using TACHYON.Invoices.Periods;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.Invoices.SubmitInvoices.Dto;
using TACHYON.Invoices.Transactions;
using TACHYON.Notifications;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Invoices.Groups
{
    [AbpAuthorize(AppPermissions.Pages_Invoices_SubmitInvoices)]
    public class SubmitInvoicesAppService : TACHYONAppServiceBase, ISubmitInvoiceAppService
    {
        private readonly IRepository<InvoicePeriod> _PeriodRepository;
        private readonly IRepository<SubmitInvoice, long> _SubmitInvoiceRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly CommonManager _commonManager;
        private readonly IRepository<InvoicePaymentMethod> _invoicePaymentMethodRepository;
        private readonly IFeatureChecker _featureChecker;
        private readonly UserManager _userManager;
        private readonly IAppNotifier _appNotifier;
        private readonly InvoiceManager _invoiceManager;
        private readonly IExcelExporterManager<SubmitInvoiceListDto> _excelExporterManager;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly IExcelExporterManager<InvoiceItemDto> _excelExporterInvoiceItemManager;
        private readonly BalanceManager _balanceManager;
        private readonly TransactionManager _transactionManager;
        private readonly IRepository<DynamicInvoice, long> _DynamicInvoiceRepository;


        public SubmitInvoicesAppService(
            BalanceManager BalanceManager,
            IRepository<InvoicePeriod> PeriodRepository,
            IRepository<SubmitInvoice, long> SubmitInvoiceRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            CommonManager commonManager,
            UserManager userManager,
            IAppNotifier appNotifier,
            InvoiceManager invoiceManager,
            IExcelExporterManager<SubmitInvoiceListDto> excelExporterManager,
            IRepository<DocumentFile, Guid> documentFileRepository, IExcelExporterManager<InvoiceItemDto> excelExporterInvoiceItemManager, IRepository<InvoicePaymentMethod> invoicePaymentMethodRepository, IFeatureChecker featureChecker, BalanceManager balanceManager, TransactionManager transactionManager, IRepository<DynamicInvoice, long> dynamicInvoiceRepository)
        {
            _PeriodRepository = PeriodRepository;
            _SubmitInvoiceRepository = SubmitInvoiceRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _commonManager = commonManager;
            _userManager = userManager;
            _appNotifier = appNotifier;
            _invoiceManager = invoiceManager;
            _excelExporterManager = excelExporterManager;
            _documentFileRepository = documentFileRepository;
            _excelExporterInvoiceItemManager = excelExporterInvoiceItemManager;
            _invoicePaymentMethodRepository = invoicePaymentMethodRepository;
            _featureChecker = featureChecker;
            _balanceManager = balanceManager;
            _transactionManager = transactionManager;
            _DynamicInvoiceRepository = dynamicInvoiceRepository;
        }


        public async Task<LoadResult> GetAllSubmitInvoices(GetAllSubmitInvoicesInput input)
        {
            DisableTenancyFilters();

            var query = _SubmitInvoiceRepository
                .GetAll()
                .AsNoTracking()
                .WhereIf(AbpSession.TenantId.HasValue && !await IsEnabledAsync(AppFeatures.TachyonDealer),
                    i => i.TenantId == AbpSession.TenantId.Value)
                .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), i => true)
                .ProjectTo<SubmitInvoiceListDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(query, input.LoadData);
        }


        public async Task<SubmitInvoiceInfoDto> GetById(EntityDto input)
        {
            var SubmitInvoice = await GetSubmitInvoiceInfo(input.Id);
            List<InvoiceItemDto> Items = GetInvoiceItems(SubmitInvoice);
            var invoiceDto = ObjectMapper.Map<SubmitInvoiceInfoDto>(SubmitInvoice);
            var Admin = await _userManager.GetAdminByTenantIdAsync(SubmitInvoice.TenantId);
            invoiceDto.Items = Items;
            invoiceDto.Phone = Admin.PhoneNumber;
            invoiceDto.Email = Admin.EmailAddress;
            invoiceDto.DueDate = invoiceDto.CreationTime;
            var documnet = await _documentFileRepository.FirstOrDefaultAsync(x =>
                x.TenantId == SubmitInvoice.TenantId && x.DocumentTypeId == 14);
            if (documnet != null) invoiceDto.CR = documnet.Number;
            return invoiceDto;
        }


        [AbpAuthorize(AppPermissions.Pages_Invoices_SubmitInvoices_Claim)]
        public async Task Claim(SubmitInvoiceClaimCreateInput Input)
        {
            var submit = await GetSubmitInvoice(Input.Id);
            submit.DueDate = Clock.Now;
            int carrierInvoicePaymentMethod = default(int);
            try
            {
                carrierInvoicePaymentMethod = int.Parse(_featureChecker.GetValue(submit.Tenant.Id, AppFeatures.InvoicePaymentMethodCrarrier));
            }
            catch
            {
                throw new UserFriendlyException(L("PleaseSelectPaymentMethodForCarrier"));
            }
            var paymentType = await _invoicePaymentMethodRepository.FirstOrDefaultAsync(x => x.Id == carrierInvoicePaymentMethod);
            if (paymentType.PaymentType == PaymentMethod.InvoicePaymentType.Days)
                submit.DueDate = Clock.Now.AddDays(paymentType.InvoiceDueDateDays);

            if (submit.Status == SubmitInvoiceStatus.Claim || submit.Status == SubmitInvoiceStatus.Accepted) return;

            var document = await _commonManager.UploadDocumentAsBase64(ObjectMapper.Map<DocumentUpload>(Input), AbpSession.TenantId);
            submit.Status = SubmitInvoiceStatus.Claim;
            submit.RejectedReason = string.Empty;
            ObjectMapper.Map(document, submit);

            var admin = await _userManager.GetAdminHostAsync();
            await _appNotifier.SubmitInvoiceOnClaim(new Abp.UserIdentifier(admin.TenantId, admin.Id), submit);
        }


        public async Task Accepted(long id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer);
            DisableTenancyFilters();
            var invoice = await _SubmitInvoiceRepository
                .GetAll()
                .Include(g => g.Tenant)
                .Include(g => g.Trips)
                .ThenInclude(x => x.ShippingRequestTripFK)
                .FirstOrDefaultAsync(g => g.Id == id && g.Status == SubmitInvoiceStatus.Claim);
            if (invoice != null)
            {
                invoice.Status = SubmitInvoiceStatus.Accepted;
                await _appNotifier.SubmitInvoiceOnAccepted(
                    new UserIdentifier(invoice.TenantId,
                        (await _userManager.GetAdminByTenantIdAsync(invoice.TenantId)).Id), invoice);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_SubmitInvoices_Rejected)]
        public async Task Rejected(SubmitInvoiceRejectedInput Input)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer);
            DisableTenancyFilters();
            var submit = await _SubmitInvoiceRepository.GetAllIncluding(g => g.Trips, g => g.Tenant)
                .SingleAsync(g => g.Id == Input.Id && g.Status == SubmitInvoiceStatus.Claim);

            if (submit != null)
            {
                submit.Status = SubmitInvoiceStatus.Rejected;
                submit.RejectedReason = Input.Reason;
                await _appNotifier.SubmitInvoiceOnRejected(
                    new UserIdentifier(submit.TenantId,
                        (await _userManager.GetAdminByTenantIdAsync(submit.TenantId)).Id), submit);
            }
        }


        [AbpAuthorize(AppPermissions.Pages_Invoices_SubmitInvoices)]
        public async Task<FileDto> GetFileDto(long GroupId)
        {
            DisableTenancyFilters();
            var documentFile = await _SubmitInvoiceRepository.FirstOrDefaultAsync(g =>
                g.Id == GroupId && g.Status != SubmitInvoices.SubmitInvoiceStatus.New);
            if (documentFile == null)
                throw new UserFriendlyException(L("TheRequestNotFound"));

            return await _commonManager.GetDocument(ObjectMapper.Map<IHasDocument>(documentFile));
        }

        public async Task<FileDto> Exports(SubmitInvoiceFilterInput input)
        {
            string[] HeaderText;
            Func<SubmitInvoiceListDto, object>[] propertySelectors;
            if (!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                HeaderText = new string[]
                {
                    "SubmitInvoiceNo", "CompanyName", "Interval", "TotalAmount", "CreationTime", "Status"
                };
                propertySelectors = new Func<SubmitInvoiceListDto, object>[]
                {
                    _ => _.ReferencNumber, _ => _.TenantName, _ => _.Period, _ => _.TotalAmount,
                    _ => _.CreationTime.ToShortDateString(), _ => _.StatusTitle
                };
            }
            else
            {
                HeaderText = new string[]
                {
                    "SubmitInvoiceNo", "CompanyName", "Interval", "TotalAmount", "CreationTime", "Status"
                };
                propertySelectors = new Func<SubmitInvoiceListDto, object>[]
                {
                    _ => _.ReferencNumber, _ => _.Period, _ => _.TotalAmount,
                    _ => _.CreationTime.ToShortDateString(), _ => _.StatusTitle
                };
            }


            return await _commonManager.ExecuteMethodIfHostOrTenantUsers(async () =>
            {
                var InvoiceListDto = ObjectMapper.Map<List<SubmitInvoiceListDto>>(await GetSubmitInvoices(input));
                return _excelExporterManager.ExportToFile(InvoiceListDto, "SubmitInvoices", HeaderText,
                    propertySelectors);
            });
        }


        [AbpAuthorize(AppPermissions.Pages_Invoices_SubmitInvoices)]
        public async Task<FileDto> ExportItems(long id)
        {
            var SubmitInvoice = await GetSubmitInvoiceInfo(id);
            List<InvoiceItemDto> Items = GetInvoiceItems(SubmitInvoice);

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

            return _excelExporterInvoiceItemManager.ExportToFile(Items, "SubmitInvoices", HeaderText,
                propertySelectors);
        }
        public async Task<bool> MakeSubmitInvoicePaid(long SubmitinvoiceId)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer);

            var Invoice = await GetSubmitInvoice(SubmitinvoiceId);
            if (Invoice != null && Invoice.Status != SubmitInvoiceStatus.Paid)
            {
                return await _commonManager.ExecuteMethodIfHostOrTenantUsers(async () =>
                {
                    await _balanceManager.AddBalanceToCarrier(Invoice.TenantId, +Invoice.TotalAmount);
                    Invoice.Status = SubmitInvoiceStatus.Paid;
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
        public async Task MakeSubmitInvoiceUnPaid(long SubmitinvoiceId)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer);

            var Invoice = await GetSubmitInvoice(SubmitinvoiceId);
            if (Invoice != null && Invoice.Status != SubmitInvoiceStatus.UnPaid)
            {
                await _balanceManager.AddBalanceToCarrier(Invoice.TenantId, -Invoice.TotalAmount);
                await _transactionManager.Delete(Invoice.Id, ChannelType.Invoices);
                Invoice.Status = SubmitInvoiceStatus.UnPaid;
            }
        }
        #region Heleper

        private async Task<IOrderedQueryable<SubmitInvoice>> GetSubmitInvoices(SubmitInvoiceFilterInput input)
        {
            var query = _SubmitInvoiceRepository
                .GetAll()
                .AsNoTracking()
                .Include(i => i.Tenant)
                .Include(i => i.InvoicePeriodsFK)
                .WhereIf(input.TenantId.HasValue, i => i.TenantId == input.TenantId)
                .WhereIf(AbpSession.TenantId.HasValue && !await IsEnabledAsync(AppFeatures.TachyonDealer),
                    i => i.TenantId == AbpSession.TenantId.Value)
                .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), i => true)
                .WhereIf(input.Status.HasValue, i => i.Status == input.Status)
                .WhereIf(input.PeriodId.HasValue, i => i.PeriodId == input.PeriodId)
                .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue,
                    i => i.CreationTime >= input.FromDate && i.CreationTime < input.ToDate)
                .OrderBy(!string.IsNullOrEmpty(input.Sorting) ? input.Sorting : "status asc");
            return query;
        }

        private async Task<SubmitInvoice> GetSubmitInvoice(long GroupId)
        {
            DisableTenancyFilters();
            return await _SubmitInvoiceRepository.GetAll()
                .Include(x => x.Tenant)
                .WhereIf(AbpSession.TenantId.HasValue && !await IsEnabledAsync(AppFeatures.TachyonDealer), e => e.TenantId == AbpSession.TenantId.Value)
                .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), e => true)
                .FirstOrDefaultAsync(g => g.Id == GroupId);
        }
        private async Task<SubmitInvoice> GetSubmitInvoiceInfo(long id)
        {
            DisableTenancyFilters();
            //todo this needs enhancement to get item as IQuerable, then fill as anynomous fields without all includes
            var SubmitInvoice = await _SubmitInvoiceRepository
                .GetAll()
                .AsNoTracking()
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
                .ThenInclude(r => r.DestinationCityFk)
                .ThenInclude(r => r.Translations)
                .Include(i => i.Trips)
                .ThenInclude(r => r.ShippingRequestTripFK)
                .ThenInclude(r => r.AssignedTruckFk)
                .Include(i => i.Trips)
                .ThenInclude(r => r.ShippingRequestTripFK)
                .ThenInclude(r => r.AssignedTruckFk)
                .ThenInclude(r => r.TrucksTypeFk)
                .ThenInclude(r => r.Translations)
                .Include(i => i.Trips)
                .Include(r=>r.Penalties)
                .ThenInclude(r=>r.ShippingRequestTripFK)
                .ThenInclude(i => i.ShippingRequestFk)
                .ThenInclude(r => r.OriginCityFk)
                .ThenInclude(r => r.Translations)
                .Include(r => r.Penalties)
                .ThenInclude(r => r.ShippingRequestTripFK)
                .ThenInclude(i => i.ShippingRequestFk)
                .ThenInclude(r => r.DestinationCityFk)
                .ThenInclude(r => r.Translations)
                .Include(r => r.Penalties)
                .ThenInclude(r => r.ShippingRequestTripFK)
                .ThenInclude(r => r.AssignedTruckFk)
                .ThenInclude(r => r.TrucksTypeFk)
                .ThenInclude(r => r.Translations)
                .WhereIf(AbpSession.TenantId.HasValue && !await IsEnabledAsync(AppFeatures.TachyonDealer),
                    i => i.TenantId == AbpSession.TenantId.Value)
                .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), i => true)
                .FirstOrDefaultAsync(i => i.Id == id);
            if (SubmitInvoice == null) throw new UserFriendlyException(L("TheSubmitInvoiceNotFound"));
            return SubmitInvoice;
        }

        private List<InvoiceItemDto> GetInvoiceItems(SubmitInvoice SubmitInvoice)
        {
            if(SubmitInvoice.Channel== InvoiceChannel.Penalty)
            {
                return GetPenaltyInvoiceItems(SubmitInvoice);
            }

            else if (SubmitInvoice.Channel == InvoiceChannel.DynamicInvoice)
            {
                return GetDynamicInvoiceItems(SubmitInvoice);
            }
            var TotalItem = SubmitInvoice.Trips.Count +
                            SubmitInvoice.Trips.Sum(v => v.ShippingRequestTripFK.ShippingRequestTripVases.Count);
            int Sequence = 1;
            List<InvoiceItemDto> Items = new List<InvoiceItemDto>();
            SubmitInvoice.Trips.ToList().ForEach(trip =>
            {
                int VasCounter = 0;
                Items.Add(new InvoiceItemDto
                {
                    Sequence = $"{Sequence}/{TotalItem}",
                    SubTotalAmount = trip.ShippingRequestTripFK.SubTotalAmount.Value,
                    VatAmount = trip.ShippingRequestTripFK.VatAmount.Value,
                    TotalAmount = trip.ShippingRequestTripFK.TotalAmount.Value,
                    WayBillNumber = trip.ShippingRequestTripFK.WaybillNumber.ToString(),
                    TruckType = ObjectMapper.Map<TrucksTypeDto>(trip.ShippingRequestTripFK.AssignedTruckFk.TrucksTypeFk).TranslatedDisplayName,
                    Source = ObjectMapper.Map<CityDto>(trip.ShippingRequestTripFK.ShippingRequestFk.OriginCityFk)?.TranslatedDisplayName ?? trip.ShippingRequestTripFK.ShippingRequestFk.OriginCityFk.DisplayName,
                    Destination = ObjectMapper.Map<CityDto>(trip.ShippingRequestTripFK.ShippingRequestFk.DestinationCityFk)?.TranslatedDisplayName ?? trip.ShippingRequestTripFK.ShippingRequestFk.DestinationCityFk.DisplayName,
                    DateWork = trip.ShippingRequestTripFK.EndWorking.HasValue ? trip.ShippingRequestTripFK.EndWorking.Value.ToString("dd MMM, yyyy") : "",
                    Remarks = trip.ShippingRequestTripFK.ShippingRequestFk.RouteTypeId == Shipping.ShippingRequests.ShippingRequestRouteType.MultipleDrops ?
                    L("TotalOfDrop", trip.ShippingRequestTripFK.ShippingRequestFk.NumberOfDrops) : ""
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
                        SubTotalAmount = vas.SubTotalAmount.Value,
                        VatAmount = vas.VatAmount.Value,
                        TotalAmount = vas.TotalAmount.Value,
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

        private List<InvoiceItemDto> GetPenaltyInvoiceItems(SubmitInvoice submitInvoice)
        {
            var TotalItem = submitInvoice.Penalties.Count;

            int Sequence = 1;
            List<InvoiceItemDto> Items = new List<InvoiceItemDto>();
            submitInvoice.Penalties.ToList().ForEach(penalty =>
            {
                Items.Add(new InvoiceItemDto
                {
                    Sequence = $"{Sequence}/{TotalItem}",
                    SubTotalAmount = penalty.AmountPreCommestion,
                    VatAmount = penalty.VatAmount,
                    TotalAmount = penalty.AmountPreCommestion+penalty.VatAmount,
                    WayBillNumber = penalty.ShippingRequestTripFK?.WaybillNumber.ToString(),
                    TruckType = penalty.ShippingRequestTripId !=null && penalty.ShippingRequestTripFK.AssignedTruckId  !=null
                    ? ObjectMapper.Map<TrucksTypeDto>(penalty.ShippingRequestTripFK.AssignedTruckFk.TrucksTypeFk).TranslatedDisplayName
                    : "-",
                    Source = penalty.ShippingRequestTripId != null 
                    ? ObjectMapper.Map<CityDto>(penalty.ShippingRequestTripFK.ShippingRequestFk.OriginCityFk)?.TranslatedDisplayName ?? penalty.ShippingRequestTripFK.ShippingRequestFk.OriginCityFk.DisplayName
                    : "-",
                    Destination = penalty.ShippingRequestTripId != null 
                    ? ObjectMapper.Map<CityDto>(penalty.ShippingRequestTripFK.ShippingRequestFk.DestinationCityFk)?.TranslatedDisplayName ?? penalty.ShippingRequestTripFK.ShippingRequestFk.DestinationCityFk.DisplayName
                    : "-",
                    DateWork = penalty.ShippingRequestTripId != null 
                    ? penalty.ShippingRequestTripFK.EndWorking.HasValue ? penalty.ShippingRequestTripFK.EndWorking.Value.ToString("dd MMM, yyyy") : ""
                    : "-",
                    Remarks = penalty.ShippingRequestTripId != null  
                    ? penalty.ShippingRequestTripFK.ShippingRequestFk.RouteTypeId == Shipping.ShippingRequests.ShippingRequestRouteType.MultipleDrops ?
                    L("TotalOfDrop", penalty.ShippingRequestTripFK.ShippingRequestFk.NumberOfDrops) : ""
                    : "-"
                });
                Sequence++;
            });
            return Items;
        }


        private List<InvoiceItemDto> GetDynamicInvoiceItems(SubmitInvoice submitInvoice)
        {
            var dynamicInvoice = _DynamicInvoiceRepository.GetAll()
                .Include(x => x.Items)
                .ThenInclude(x=>x.Truck)
                .ThenInclude(x=>x.TrucksTypeFk)
                .Include(x => x.Items)
                .ThenInclude(x=>x.ShippingRequestTrip)
                .ThenInclude(x=>x.AssignedTruckFk)
                .ThenInclude(x=>x.TrucksTypeFk)
                .ThenInclude(x=>x.Translations)
                .Include(x => x.Items)
                .ThenInclude(x => x.ShippingRequestTrip)
                .ThenInclude(x=>x.ShippingRequestFk)
                .ThenInclude(x=>x.OriginCityFk)
                .ThenInclude(x=>x.Translations)
                .Include(x => x.Items)
                .ThenInclude(x => x.ShippingRequestTrip)
                .ThenInclude(x => x.ShippingRequestFk)
                .ThenInclude(x => x.DestinationCityFk)
                .ThenInclude(x => x.Translations)
                .FirstOrDefault(x => x.SubmitInvoiceId == submitInvoice.Id);

            var TotalItem = dynamicInvoice.Items.Count;

            int Sequence = 1;
            List<InvoiceItemDto> Items = new List<InvoiceItemDto>();
            dynamicInvoice.Items.ToList().ForEach(item =>
            {
                Items.Add(new InvoiceItemDto
                {
                    Sequence = $"{Sequence}/{TotalItem}",
                    SubTotalAmount = item.Price,
                    VatAmount = item.VatAmount,
                    TotalAmount = item.TotalAmount,
                    WayBillNumber = item.ShippingRequestTrip?.WaybillNumber.ToString(),
                    //TruckType = item.ShippingRequestTrip != null
                    //? ObjectMapper.Map<TrucksTypeDto>(item.ShippingRequestTrip.AssignedTruckFk.TrucksTypeFk).TranslatedDisplayName ?? item.ShippingRequestTrip.ShippingRequestFk.AssignedTruckFk.TrucksTypeFk.DisplayName
                    //: ObjectMapper.Map<TrucksTypeDto>(item.Truck?.TrucksTypeFk).TranslatedDisplayName ?? item.Truck?.TrucksTypeFk.DisplayName,
                    Source = item.ShippingRequestTrip != null
                    ? ObjectMapper.Map<CityDto>(item.ShippingRequestTrip.ShippingRequestFk.OriginCityFk)?.TranslatedDisplayName ?? item.ShippingRequestTrip.ShippingRequestFk.OriginCityFk.DisplayName
                    : ObjectMapper.Map<CityDto>(item.OriginCity)?.TranslatedDisplayName ?? item.OriginCity?.DisplayName,
                    Destination = item.ShippingRequestTrip!= null
                    ? ObjectMapper.Map<CityDto>(item.ShippingRequestTrip.ShippingRequestFk.DestinationCityFk)?.TranslatedDisplayName ?? item.ShippingRequestTrip.ShippingRequestFk.DestinationCityFk.DisplayName
                    : ObjectMapper.Map<CityDto>(item.DestinationCity)?.TranslatedDisplayName ?? item.DestinationCity?.DisplayName,
                    DateWork = item.ShippingRequestTrip != null
                    ? item.ShippingRequestTrip.EndWorking.HasValue ? item.ShippingRequestTrip.EndWorking.Value.ToString("dd MMM, yyyy") : ""
                    : item.WorkDate!=null ?item.WorkDate.Value.ToString("dd MMM, yyyy") :"",
                    Remarks = item.ShippingRequestTrip != null
                    ? item.ShippingRequestTrip.ShippingRequestFk.RouteTypeId == ShippingRequestRouteType.MultipleDrops ?
                    L("TotalOfDrop", item.ShippingRequestTrip.ShippingRequestFk.NumberOfDrops) : "-"
                    : L("TotalOfDrop", item.Quantity?.ToString())
                });
                Sequence++;
            });
            return Items;
        }
        #endregion
    }
}