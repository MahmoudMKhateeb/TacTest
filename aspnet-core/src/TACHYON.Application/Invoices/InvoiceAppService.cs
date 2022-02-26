using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Threading;
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
using TACHYON.Configuration;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Dto;
using TACHYON.Exporting;
using TACHYON.Features;
using TACHYON.Invoices.Balances;
using TACHYON.Invoices.Dto;
using TACHYON.Invoices.Periods;
using TACHYON.Invoices.Transactions;
using TACHYON.ShippingRequestVases;
using TACHYON.Trucks.TrucksTypes.Dtos;


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

        public InvoiceAppService(
            IRepository<Invoice, long> invoiceRepository,
            CommonManager commonManager,
            BalanceManager balanceManager,
            UserManager userManager,
            InvoiceManager invoiceManager,
            TransactionManager transactionManager,
            IExcelExporterManager<InvoiceListDto> excelExporterManager, IRepository<ShippingRequestVas, long> shippingRequestVasesRepository, IRepository<DocumentFile, Guid> documentFileRepository, IExcelExporterManager<InvoiceItemDto> excelExporterInvoiceItemManager)

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
        }


        public async Task<LoadResult> GetAll(string filter)

        {
            var query = _invoiceRepository
                            .GetAll()
                            .ProjectTo<InvoiceListDto>(AutoMapperConfigurationProvider)
                            .AsNoTracking();
            if (!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                DisableTenancyFilters();
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
            var documnet = await _documentFileRepository.FirstOrDefaultAsync(x => x.TenantId == invoice.TenantId && x.DocumentTypeId == 14);
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
                            .FirstOrDefaultAsync(i => i.Id == invoiceId);
            if (invoice == null) throw new UserFriendlyException(L("TheInvoiceNotFound"));

            return invoice;
        }

        private List<InvoiceItemDto> GetInvoiceItems(Invoice invoice)
        {
            var TotalItem = invoice.Trips.Count + invoice.Trips.Sum(v => v.ShippingRequestTripFK.ShippingRequestTripVases.Count);
            int Sequence = 1;
            List<InvoiceItemDto> Items = new List<InvoiceItemDto>();
            invoice.Trips.ToList().ForEach(trip =>
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


        public async Task OnDemand(int Id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer);
            DisableTenancyFilters();
            var tenant = await TenantManager.GetByIdAsync(Id);

            // if (tenant == null || tenant.Name == AppConsts.ShipperEditionName) throw new UserFriendlyException(L("TheTenantSelectedIsNotShipper"));
            await _invoiceManager.GenertateInvoiceOnDeman(tenant);
        }
        #region Reports
        public IEnumerable<InvoiceInfoDto> GetInvoiceReportInfo(long invoiceId)
        {

            var bankNameArabic = SettingManager.GetSettingValue(AppSettings.Invoice.BankNameArabic);
            var bankNameEnglish = SettingManager.GetSettingValue(AppSettings.Invoice.BankNameEnglish);

            DisableTenancyFilters();
            var invoice = _invoiceRepository
                .GetAll()
                .Include(i => i.InvoicePeriodsFK)
                .Include(i => i.Tenant)
                .FirstOrDefault(i => i.Id == invoiceId);

            if (invoice == null) throw new UserFriendlyException(L("TheInvoiceNotFound"));

            var invoiceDto = ObjectMapper.Map<InvoiceInfoDto>(invoice);

            var admin = AsyncHelper.RunSync(() => _userManager.GetAdminByTenantIdAsync(invoice.TenantId));

            invoiceDto.Phone = admin.PhoneNumber;
            invoiceDto.Email = admin.EmailAddress;
            invoiceDto.Attn = admin.FullName;
            invoiceDto.BankNameArabic = bankNameArabic;
            invoiceDto.BankNameEnglish = bankNameEnglish;
            var document = AsyncHelper.RunSync(() => _documentFileRepository.FirstOrDefaultAsync(x => x.TenantId == invoice.TenantId && x.DocumentTypeId == 14));
            if (document != null) invoiceDto.CR = document.Number;
            var documentVat = AsyncHelper.RunSync(() => _documentFileRepository.FirstOrDefaultAsync(x => x.TenantId == invoice.TenantId && x.DocumentTypeId == 15));
            if (document != null) invoiceDto.TenantVatNumber = documentVat.Number;
            return new List<InvoiceInfoDto>() { invoiceDto };
        }

        public IEnumerable<InvoiceItemDto> GetInvoiceShippingRequestsReportInfo(long invoiceId)
        {
            DisableTenancyFilters();
            var invoice = AsyncHelper.RunSync(() => GetInvoiceInfo(invoiceId));

            if (invoice == null) throw new UserFriendlyException(L("TheInvoiceNotFound"));
            var TotalItem = invoice.Trips.Count + invoice.Trips.SelectMany(v => v.ShippingRequestTripFK.ShippingRequestTripVases).Count();
            int Sequence = 1;
            List<InvoiceItemDto> Items = new List<InvoiceItemDto>();
            invoice.Trips.ToList().ForEach(trip =>
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
                    DateWork = trip.ShippingRequestTripFK.EndTripDate.HasValue ? trip.ShippingRequestTripFK.EndTripDate.Value.ToString("dd/MM/yyyy") : trip.InvoiceFK.CreationTime.ToString("dd/MM/yyyy"),
                    Remarks = trip.ShippingRequestTripFK.ShippingRequestFk.RouteTypeId == Shipping.ShippingRequests.ShippingRequestRouteType.MultipleDrops ?
                    L("TotalOfDrop", trip.ShippingRequestTripFK.ShippingRequestFk.NumberOfDrops) : "",
                    ContainerNumber = trip.ShippingRequestTripFK.CanBePrinted ? trip.ShippingRequestTripFK.ContainerNumber ?? "-" : "-",
                    RoundTrip = trip.ShippingRequestTripFK.CanBePrinted ? trip.ShippingRequestTripFK.RoundTrip ?? "-" : "-",
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
                HeaderText = new string[] { "InvoiceNumber", "CompanyName", "Interval", "AccountInvoiceType", "TotalAmount", "DueDate", "CreationTime", "Status" };
                propertySelectors = new Func<InvoiceListDto, object>[] { _ => _.InvoiceNumber, _ => _.TenantName, _ => _.Period, _ => _.AccountTypeTitle, _ => _.TotalAmount, _ => _.DueDate.ToShortDateString(), _ => _.CreationTime.ToShortDateString(), _ => _.IsPaid ? "Paid" : "UnPaid" };
            }
            else
            {
                HeaderText = new string[] { "InvoiceNumber", "Interval", "TotalAmount", "DueDate", "CreationTime", "Status" };
                propertySelectors = new Func<InvoiceListDto, object>[] { _ => _.InvoiceNumber, _ => _.Period, _ => _.TotalAmount, _ => _.DueDate.ToShortDateString(), _ => _.CreationTime.ToShortDateString(), _ => _.IsPaid ? "Paid" : "UnPaid" };

            }
            return await _commonManager.ExecuteMethodIfHostOrTenantUsers(async () =>
               {
                   var InvoiceListDto = ObjectMapper.Map<List<InvoiceListDto>>(await GetInvoices(input));
                   return _excelExporterManager.ExportToFile(InvoiceListDto, "Invoices", HeaderText, propertySelectors);
               }

            );

        }


        public async Task<FileDto> ExportItems(long id)
        {
            var invoice = await GetInvoiceInfo(id);
            List<InvoiceItemDto> Items = GetInvoiceItems(invoice);

            var HeaderText = new string[] { "Sequence", "Date", "WaybillNumber", "CityOrigin", "DestinationDelivery", "TruckType", "Price", "Vat", "Total", "Quantity" };
            var propertySelectors = new Func<InvoiceItemDto, object>[] { _ => _.Sequence, _ => _.DateWork, _ => _.WayBillNumber, _ => _.Source, _ => _.Destination, _ => _.TruckType
          ,_=> _.SubTotalAmount,_=> _.VatAmount,_=> _.TotalAmount,_=> _.Remarks };

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
                .WhereIf(AbpSession.TenantId.HasValue && !await IsEnabledAsync(AppFeatures.TachyonDealer), e => e.TenantId == AbpSession.TenantId.Value)
                .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), e => true)
                .WhereIf(input.AccountType.HasValue, e => e.AccountType == input.AccountType)
                .WhereIf(input.IsPaid.HasValue, e => e.IsPaid == input.IsPaid)
                .WhereIf(input.TenantId.HasValue, i => i.TenantId == input.TenantId)
                .WhereIf(input.PeriodId.HasValue, i => i.PeriodId == input.PeriodId)
                .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue, i => i.CreationTime >= input.FromDate && i.CreationTime < input.ToDate)
                .WhereIf(input.DueFromDate.HasValue && input.DueToDate.HasValue, i => i.DueDate >= input.DueFromDate && i.DueDate < input.DueToDate)
                .AsNoTracking();

        }

        private async Task<Invoice> GetInvoice(long invoiceId)
        {
            DisableTenancyFilters();
            return await _invoiceRepository
                  .GetAll()
                  .WhereIf(AbpSession.TenantId.HasValue && !await IsEnabledAsync(AppFeatures.TachyonDealer), e => e.TenantId == AbpSession.TenantId.Value)
                  .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), e => true)
                  .FirstOrDefaultAsync(i => i.Id == invoiceId);
        }



        #endregion
    }
}