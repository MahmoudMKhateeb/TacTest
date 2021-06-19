using Abp;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
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
using TACHYON.Exporting;
using TACHYON.Features;
using TACHYON.Invoices.Balances;
using TACHYON.Invoices.Dto;
using TACHYON.Invoices.Groups.Dto;
using TACHYON.Invoices.Periods;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.Invoices.SubmitInvoices.Dto;
using TACHYON.Notifications;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Invoices.Groups
{
    [AbpAuthorize(AppPermissions.Pages_Invoices_SubmitInvoices)]

    public class SubmitInvoicesAppService : TACHYONAppServiceBase,ISubmitInvoiceAppService
    {
        private readonly IRepository<InvoicePeriod> _PeriodRepository;
        private readonly IRepository<SubmitInvoice, long> _SubmitInvoiceRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly CommonManager _commonManager;
        private readonly UserManager _userManager;
        private readonly IAppNotifier _appNotifier;
        private readonly InvoiceManager _invoiceManager;
        private readonly IExcelExporterManager<SubmitInvoiceListDto> _excelExporterManager;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;


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
            IRepository<DocumentFile, Guid> documentFileRepository)
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
        }


        public async Task<PagedResultDto<SubmitInvoiceListDto>> GetAll(SubmitInvoiceFilterInput input)
        {
            DisableTenancyFilters();
            var query = await GetSubmitInvoices(input);
            var pages = query.PageBy(input);

            var totalCount = await query.CountAsync();

            return new PagedResultDto<SubmitInvoiceListDto>(
                totalCount,
                ObjectMapper.Map<List<SubmitInvoiceListDto>>(pages)
            );

        }


        public async Task<SubmitInvoiceInfoDto> GetById(EntityDto input)
        {
            DisableTenancyFilters();
            var SubmitInvoice = await GetSubmitInvoiceInfo(input.Id);


            if (SubmitInvoice == null) throw new UserFriendlyException(L("TheSubmitInvoiceNotFound"));

            var TotalItem = SubmitInvoice.Trips.Count + SubmitInvoice.Trips.Select(v => v.ShippingRequestTripFK.ShippingRequestTripVases).Count();
            int Sequence = 1;
            List<InvoiceItemDto> Items = new List<InvoiceItemDto>();
            SubmitInvoice.Trips.ToList().ForEach(trip =>
            {
                int VasCounter = 0;
                Items.Add(new InvoiceItemDto
                {
                    Sequence = $"{Sequence}/{TotalItem}",
                    SubTotalAmount = AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier) ? trip.ShippingRequestTripFK.SubTotalAmount.Value : trip.ShippingRequestTripFK.SubTotalAmountWithCommission.Value,
                    VatAmount = AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier) ? trip.ShippingRequestTripFK.VatAmount.Value : trip.ShippingRequestTripFK.VatAmountWithCommission.Value,
                    TotalAmount = AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier) ? trip.ShippingRequestTripFK.TotalAmount.Value : trip.ShippingRequestTripFK.TotalAmountWithCommission.Value,
                    WayBillNumber = trip.ShippingRequestTripFK.WaybillNumber.ToString(),
                    TruckType = ObjectMapper.Map<TrucksTypeDto>(trip.ShippingRequestTripFK.AssignedTruckFk.TrucksTypeFk).TranslatedDisplayName,
                    Source = ObjectMapper.Map<CityDto>(trip.ShippingRequestTripFK.ShippingRequestFk.OriginCityFk)?.TranslatedDisplayName ?? trip.ShippingRequestTripFK.ShippingRequestFk.OriginCityFk.DisplayName,
                    Destination = ObjectMapper.Map<CityDto>(trip.ShippingRequestTripFK.ShippingRequestFk.DestinationCityFk)?.TranslatedDisplayName ?? trip.ShippingRequestTripFK.ShippingRequestFk.DestinationCityFk.DisplayName,
                    DateWork = trip.ShippingRequestTripFK.ShippingRequestFk.EndTripDate.HasValue ? trip.ShippingRequestTripFK.ShippingRequestFk.EndTripDate.Value.ToString("dd MMM, yyyy") : "",
                    Remarks = trip.ShippingRequestTripFK.ShippingRequestFk.RouteTypeId == Shipping.ShippingRequests.ShippingRequestRouteType.MultipleDrops ?
                    L("TotalOfDrop", trip.ShippingRequestTripFK.ShippingRequestFk.NumberOfDrops) : ""
                });
                Sequence++;
                if (trip.ShippingRequestTripFK.ShippingRequestTripVases != null && trip.ShippingRequestTripFK.ShippingRequestTripVases.Count > 1)
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
                        SubTotalAmount = AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier) ? vas.SubTotalAmount.Value : vas.SubTotalAmountWithCommission.Value,
                        VatAmount = AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier) ? vas.VatAmount.Value : vas.VatAmountWithCommission.Value,
                        TotalAmount = AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier) ? vas.TotalAmount.Value : vas.TotalAmountWithCommission.Value,
                        WayBillNumber = waybillnumber,
                        TruckType = L("InvoiceVasType", vas.ShippingRequestVasFk.VasFk.Name),
                        Source = "-",
                        Destination = "-",
                        DateWork = "-",
                        Remarks = vas.Quantity > 1 ? $"{vas.Quantity}" : ""
                    };
                    Items.Add(item);

                    Sequence++;
                }

            });


            var invoiceDto = ObjectMapper.Map<SubmitInvoiceInfoDto>(SubmitInvoice);
            var Admin = await _userManager.GetAdminByTenantIdAsync(SubmitInvoice.TenantId);
            invoiceDto.Items = Items;
            invoiceDto.Phone = Admin.PhoneNumber;
            invoiceDto.Email = Admin.EmailAddress;
            invoiceDto.DueDate = invoiceDto.CreationTime;
            DisableTenancyFilters();
            var documnet = await _documentFileRepository.FirstOrDefaultAsync(x => x.TenantId == SubmitInvoice.TenantId && x.DocumentTypeId == 14);
            if (documnet != null) invoiceDto.CR = documnet.Number;
            return invoiceDto;

        }





        [AbpAuthorize(AppPermissions.Pages_Invoices_SubmitInvoices_Claim)]
        public async Task Claim(SubmitInvoiceClaimCreateInput Input)
        {
            //CheckIfCanAccessService(true, AppFeatures.Carrier);
           // DisableTenancyFilters();
            var submit = await GetSubmitInvoice(Input.Id);
            if (submit.Status == SubmitInvoiceStatus.Claim || submit.Status == SubmitInvoiceStatus.Accepted) return;
            var document = await _commonManager.UploadDocumentAsBase64(ObjectMapper.Map<DocumentUpload>(Input), AbpSession.TenantId);
            submit.Status = SubmitInvoiceStatus.Claim;
            submit.RejectedReason = string.Empty;
            ObjectMapper.Map(document, submit);

            var admin = await _userManager.GetAdminHostAsync();
            await  _appNotifier.SubmitInvoiceOnClaim(new Abp.UserIdentifier(admin.TenantId, admin.Id) , submit);
        }


        //[AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_SubmitInvoices_Accepted)]
        public async Task Accepted(long id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer);
            DisableTenancyFilters();
            var invoice = await _SubmitInvoiceRepository
                .GetAll()
                .Include(g => g.Tenant)
                .Include(g => g.Trips)
                 .ThenInclude (x=>x.ShippingRequestTripFK)
                .FirstOrDefaultAsync(g => g.Id == id && g.Status== SubmitInvoiceStatus.Claim);            
            if (invoice != null)
            {

                 await _invoiceManager.GenerateCarrirInvoice(invoice);
                invoice.Status = SubmitInvoiceStatus.Accepted;
                await _appNotifier.SubmitInvoiceOnAccepted(new UserIdentifier(invoice.TenantId, (await _userManager.GetAdminByTenantIdAsync(invoice.TenantId)).Id), invoice);
            }
        }


        //[AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_SubmitInvoices_Rejected)]
        public async Task Rejected(SubmitInvoiceRejectedInput Input)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer);
            DisableTenancyFilters();
            var submit = await _SubmitInvoiceRepository.GetAllIncluding(g => g.Trips, g => g.Tenant).
                SingleAsync(g => g.Id == Input.Id && g.Status == SubmitInvoiceStatus.Claim);

            if (submit != null)
            {
                submit.Status = SubmitInvoiceStatus.Rejected;
                submit.RejectedReason = Input.Reason;
                await _appNotifier.SubmitInvoiceOnRejected(new UserIdentifier(submit.TenantId, (await _userManager.GetAdminByTenantIdAsync(submit.TenantId)).Id), submit);

            }
        }


        [AbpAuthorize(AppPermissions.Pages_Invoices_SubmitInvoices)]
        public async Task<FileDto> GetFileDto(long GroupId)

        {
            DisableTenancyFiltersIfHost();
            var documentFile = await _SubmitInvoiceRepository.FirstOrDefaultAsync(g => g.Id == GroupId && g.Status != SubmitInvoices.SubmitInvoiceStatus.New );
            if (documentFile == null)
            {
                throw new UserFriendlyException(L("TheRequestNotFound"));

            }

            return await _commonManager.GetDocument(ObjectMapper.Map<IHasDocument>(documentFile));

        }

        public async Task<FileDto> Exports(SubmitInvoiceFilterInput input)
        {
            string[] HeaderText;
            Func<SubmitInvoiceListDto, object>[] propertySelectors;
            if (!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer) )
            {
                HeaderText = new string[] { "SubmitInvoiceNo", "CompanyName", "Interval",  "TotalAmount", "CreationTime", "Status" };
                propertySelectors = new Func<SubmitInvoiceListDto, object>[] { _ => _.ReferenceNumber, _ => _.TenantName, _ => _.Period,  _ => _.TotalAmount, _ => _.CreationTime.ToShortDateString(), _ => _.StatusTitle};
            }
            else
            {
                HeaderText = new string[] { "SubmitInvoiceNo", "CompanyName", "Interval", "TotalAmount", "CreationTime", "Status" };
                propertySelectors = new Func<SubmitInvoiceListDto, object>[] { _ => _.ReferenceNumber, _ => _.Period, _ => _.TotalAmount, _ => _.CreationTime.ToShortDateString(), _ => _.StatusTitle };

            }



            return await _commonManager.ExecuteMethodIfHostOrTenantUsers(async () =>
            {
                var InvoiceListDto = ObjectMapper.Map<List<SubmitInvoiceListDto>>(await GetSubmitInvoices(input));
                return _excelExporterManager.ExportToFile(InvoiceListDto, "SubmitInvoices", HeaderText, propertySelectors);
            });
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
                .WhereIf(AbpSession.TenantId.HasValue && !await IsEnabledAsync(AppFeatures.TachyonDealer), i => i.TenantId == AbpSession.TenantId.Value)
                .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), i => true)
                .WhereIf(input.Status.HasValue, i => i.Status == input.Status)
                .WhereIf(input.PeriodId.HasValue, i => i.PeriodId == input.PeriodId)
                .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue, i => i.CreationTime >= input.FromDate && i.CreationTime < input.ToDate)
                .OrderBy(!string.IsNullOrEmpty(input.Sorting) ? input.Sorting : "status asc");
            return query;
        }
        private async Task<SubmitInvoice> GetSubmitInvoice(long GroupId)
        {
            return await _SubmitInvoiceRepository.FirstOrDefaultAsync(g => g.Id == GroupId);
        }

        private async Task<SubmitInvoice> GetSubmitInvoiceInfo(long id)
        {
           return await _SubmitInvoiceRepository
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
                    .WhereIf(AbpSession.TenantId.HasValue && !await IsEnabledAsync(AppFeatures.TachyonDealer), i => i.TenantId == AbpSession.TenantId.Value)
                    .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), i => true)
                            .FirstOrDefaultAsync(i => i.Id == id);
        }


        #endregion
    }
}
