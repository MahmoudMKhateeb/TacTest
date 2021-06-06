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
using TACHYON.Common;
using TACHYON.Dto;
using TACHYON.Exporting;
using TACHYON.Invoices.Balances;
using TACHYON.Invoices.Dto;
using TACHYON.Invoices.Groups.Dto;
using TACHYON.Invoices.GroupsGroups.Dto;
using TACHYON.Invoices.Periods;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.Notifications;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Invoices.Groups
{
    public class GroupPeriodAppService : TACHYONAppServiceBase,IGroupPeriodAppService
    {
        private readonly IRepository<GroupPeriod,long> _Repository;
        private readonly IRepository<InvoicePeriod> _PeriodRepository;
        private readonly IRepository<GroupPeriodInvoice, long> _GroupPeriodInvoiceRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly CommonManager _commonManager;
        private readonly UserManager _userManager;
        private readonly IAppNotifier _appNotifier;
        private readonly InvoiceManager _invoiceManager;
        private readonly IExcelExporterManager<GroupPeriodListDto> _excelExporterManager;

        public GroupPeriodAppService(
            IRepository<GroupPeriod, long> repository, 
            BalanceManager BalanceManager, 
            IRepository<InvoicePeriod> PeriodRepository,
            IRepository<GroupPeriodInvoice, long> GroupPeriodInvoiceRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            CommonManager commonManager,
            UserManager userManager,
            IAppNotifier appNotifier,
            InvoiceManager invoiceManager,
            IExcelExporterManager<GroupPeriodListDto> excelExporterManager)
        {
            _Repository = repository;
            _PeriodRepository = PeriodRepository;
            _GroupPeriodInvoiceRepository = GroupPeriodInvoiceRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _commonManager = commonManager;
            _userManager = userManager;
            _appNotifier = appNotifier;
            _invoiceManager = invoiceManager;
            _excelExporterManager = excelExporterManager;
        }


        [AbpAuthorize(AppPermissions.Pages_Invoices_SubmitInvoices)]
        public async Task<PagedResultDto<GroupPeriodListDto>> GetAll(GroupPeriodFilterInput input)
        {
            IQueryable<GroupPeriod> query= await _commonManager.ExecuteMethodIfHostOrTenantUsers(  () =>  GetGroupPeriods(input));
            var pages = query.PageBy(input);

            var totalCount = await query.CountAsync();

            return new PagedResultDto<GroupPeriodListDto>(
                totalCount,
                ObjectMapper.Map<List<GroupPeriodListDto>>(pages)
            );

        }

        [AbpAuthorize(AppPermissions.Pages_Invoices_SubmitInvoices)]

        public async Task<GroupPeriodInfoDto> GetById(EntityDto input)
        {
            var SubmitInvoice = await _commonManager.ExecuteMethodIfHostOrTenantUsers(() => GetGroupPeriodInfo(input.Id));


            if (SubmitInvoice == null) throw new UserFriendlyException(L("TheSubmitInvoiceNotFound"));

            List<SubmitInvoiceItemDto> Items = new List<SubmitInvoiceItemDto>();
            SubmitInvoice.ShippingRequests.ForEach(request =>
            {
                Items.Add(new SubmitInvoiceItemDto
                {
                    Price = request.ShippingRequests.Price,
                    TruckType = ObjectMapper.Map<TrucksTypeDto>(request.ShippingRequests.TrucksTypeFk).TranslatedDisplayName,//request.ShippingRequests.TrucksTypeFk.DisplayName,
                Source = request.ShippingRequests.OriginCityFk.DisplayName,
                    Destination = request.ShippingRequests.DestinationCityFk.DisplayName,
                    DateWork = request.ShippingRequests.StartTripDate.Value.ToString("dd MMM, yyyy"),
                    Remarks = L("TotalOfDrop", request.ShippingRequests.NumberOfDrops)
                });

                foreach (var vas in request.ShippingRequests.ShippingRequestVases)
                {
                    var item = new SubmitInvoiceItemDto
                    {
                        Price = (decimal?)vas.ActualPrice,
                        TruckType = L("InvoiceVasType", vas.VasFk.Name),
                        Source = "-",
                        Destination = "-",
                        DateWork = "-"
                    };
                    if (vas.RequestMaxAmount > 0 && vas.RequestMaxCount > 0)
                    {
                        item.Remarks = L("InvoiceVasRemarksAll", vas.RequestMaxCount, vas.RequestMaxAmount);
                    }
                    else if (vas.RequestMaxCount > 0)
                    {
                        item.Remarks = L("InvoiceVasRemarksCount", vas.RequestMaxCount);

                    }
                    else if (vas.RequestMaxAmount > 0)
                    {
                        item.Remarks = L("InvoiceVasRemarksAmount", vas.RequestMaxAmount);

                    }
                    Items.Add(item);
                }

            });

            var GroupDto = ObjectMapper.Map<GroupPeriodInfoDto>(SubmitInvoice);
            GroupDto.ShippingRequest = SubmitInvoice.ShippingRequests.Select(x => new GroupShippingRequestDto
            {
                TruckType=ObjectMapper.Map<TrucksTypeDto>(x.ShippingRequests.TrucksTypeFk).TranslatedDisplayName
            }).ToList();

            GroupDto.Items = Items;

                      DisableTenancyFilters();                
                    var user = await _userManager.Users.SingleAsync(u => u.TenantId == SubmitInvoice.TenantId && u.UserName== AbpUserBase.AdminUserName);

                    GroupDto.Email = user.EmailAddress;
                    GroupDto.Phone = user.PhoneNumber;

            return GroupDto;

        }



        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_SubmitInvoices_Delete)]

        public async Task Delete(EntityDto Input)
        {
            var Group = await _Repository.GetAllIncluding(g => g.ShippingRequests, g => g.Tenant).
             SingleAsync(g => g.Id == Input.Id);
            if (Group == null) return;
            var GroupRequest = Group.ShippingRequests.Select(r => r.RequestId).OfType<long>().ToArray();
            var Requests = _shippingRequestRepository.GetAllList(r => GroupRequest.Contains(r.Id));

            foreach (var request in Requests)
            {
                request.IsCarrierHaveInvoice = false;
            }
            await _Repository.DeleteAsync(Input.Id);       
            if (Group.Status== SubmitInvoiceStatus.Accepted)
            {
                var Invoice = await _GroupPeriodInvoiceRepository.FirstOrDefaultAsync(g => g.GroupId == Group.Id);
                if (Invoice !=null)
                {
                    await _invoiceManager.RemoveInvoiceFromRequest(Invoice.InvoiceId);
                    await _GroupPeriodInvoiceRepository.DeleteAsync(Invoice);
                }

                
            }
            else if (Group.Status == SubmitInvoiceStatus.Claim)
            {
               await _commonManager.DeleteDocument((Guid)Group.DocumentId);
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Invoices_SubmitInvoices_Claim)]
        public async Task Claim(GroupPeriodClaimCreateInput Input)
        {
            var Group = await GetGroupPeriod(Input.Id);
            if (Group.Status == SubmitInvoiceStatus.Claim || Group.Status == SubmitInvoiceStatus.Accepted) return;
            var document = await _commonManager.UploadDocumentAsBase64(ObjectMapper.Map<DocumentUpload>(Input), AbpSession.TenantId);
            Group.Status = SubmitInvoiceStatus.Claim;
            Group.RejectedReason = string.Empty;
            ObjectMapper.Map(document, Group);

            var admin = await _userManager.GetAdminHostAsync();
            await  _appNotifier.SubmitInvoiceOnClaim(new Abp.UserIdentifier(admin.TenantId, admin.Id) ,Group);
        }


        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_SubmitInvoices_Accepted)]
        public async Task Accepted(long GroupId)
        {
            var Group = await _Repository.GetAllIncluding(g => g.ShippingRequests, g => g.Tenant).
                SingleAsync(g => g.Id == GroupId && g.Status== SubmitInvoiceStatus.Claim);            
            if (Group !=null)
            {
               
               await _invoiceManager.GenerateCarrirInvoice(Group);
               Group.Status = SubmitInvoiceStatus.Accepted;
              await _appNotifier.SubmitInvoiceOnAccepted(new UserIdentifier(Group.TenantId, (await _userManager.GetAdminByTenantIdAsync(Group.TenantId)).Id), Group);
            }
        }


        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_SubmitInvoices_Rejected)]
        public async Task Rejected(SubmitInvoiceRejectedInput Input)
        {
            var Group = await _Repository.GetAllIncluding(g => g.ShippingRequests, g => g.Tenant).
                SingleAsync(g => g.Id == Input.Id && g.Status == SubmitInvoiceStatus.Claim);

            if (Group != null)
            {
                var Invoice = await _GroupPeriodInvoiceRepository.FirstOrDefaultAsync(g => g.GroupId == Group.Id);
                if (Invoice != null) {
                    await _invoiceManager.RemoveInvoiceFromRequest(Invoice.InvoiceId);
                    await _GroupPeriodInvoiceRepository.DeleteAsync(Invoice);
                } 
                Group.Status = SubmitInvoiceStatus.Rejected;
                Group.RejectedReason = Input.Reason;
                await _appNotifier.SubmitInvoiceOnRejected(new UserIdentifier(Group.TenantId, (await _userManager.GetAdminByTenantIdAsync(Group.TenantId)).Id), Group);

            }
        }


        [AbpAuthorize(AppPermissions.Pages_Invoices_SubmitInvoices)]
        public async Task<FileDto> GetFileDto(long GroupId)

        {
            DisableTenancyFiltersIfHost();
            var documentFile = await _Repository.SingleAsync(g => g.Id == GroupId && g.Status != SubmitInvoices.SubmitInvoiceStatus.None );
            if (documentFile == null)
            {
                throw new UserFriendlyException(L("TheRequestNotFound"));

            }

            return await _commonManager.GetDocument(ObjectMapper.Map<IHasDocument>(documentFile));

        }

        public async Task<FileDto> Exports(GroupPeriodFilterInput input)
        {
            string[] HeaderText;
            Func<GroupPeriodListDto, object>[] propertySelectors;
            if (AbpSession.TenantId == null)
            {
                HeaderText = new string[] { "SubmitInvoiceNo", "CompanyName", "Interval",  "TotalAmount", "CreationTime", "Status" };
                propertySelectors = new Func<GroupPeriodListDto, object>[] { _ => _.Id, _ => _.TenantName, _ => _.Period,  _ => _.Amount, _ => _.CreationTime.ToShortDateString(), _ => _.StatusTitle};
            }
            else
            {
                HeaderText = new string[] { "SubmitInvoiceNo", "CompanyName", "Interval", "TotalAmount", "CreationTime", "Status" };
                propertySelectors = new Func<GroupPeriodListDto, object>[] { _ => _.Id, _ => _.Period, _ => _.Amount, _ => _.CreationTime.ToShortDateString(), _ => _.StatusTitle };

            }



            return await _commonManager.ExecuteMethodIfHostOrTenantUsers(async () =>
            {
                var InvoiceListDto = ObjectMapper.Map<List<GroupPeriodListDto>>(await GetGroupPeriods(input));
                return _excelExporterManager.ExportToFile(InvoiceListDto, "SubmitInvoices", HeaderText, propertySelectors);
            });
        }
        #region Heleper
        private Task<IOrderedQueryable<GroupPeriod>> GetGroupPeriods(GroupPeriodFilterInput input)
        {
            var query = _Repository
                .GetAll()
                .Include(i => i.Tenant)
                .Include(i => i.InvoicePeriod)
                .WhereIf(input.TenantId.HasValue, i => i.TenantId == input.TenantId)
                .WhereIf(input.Status.HasValue, i => i.Status == input.Status)
                 .WhereIf(input.PeriodId.HasValue, i => i.PeriodId == input.PeriodId)
                .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue, i => i.CreationTime >= input.FromDate && i.CreationTime < input.ToDate)
                .OrderBy(!string.IsNullOrEmpty(input.Sorting) ? input.Sorting : "status asc");
            return Task.FromResult(query);
        }
        private async Task<GroupPeriod> GetGroupPeriod(long GroupId)
        {
            return await _Repository.SingleAsync(g => g.Id == GroupId);
        }

        private  Task<GroupPeriod> GetGroupPeriodInfo(long GroupId)
        {
           return _Repository
                            .GetAll()
                            .Include(i => i.InvoicePeriod)
                            .Include(i => i.Tenant)
                            .Include(i => i.ShippingRequests)
                             .ThenInclude(r => r.ShippingRequests)
                               .ThenInclude(r => r.OriginCityFk)
                            .Include(i => i.ShippingRequests)
                             .ThenInclude(r => r.ShippingRequests)
                               .ThenInclude(r => r.DestinationCityFk)
                            .Include(i => i.ShippingRequests)
                             .ThenInclude(r => r.ShippingRequests)
                              .ThenInclude(r => r.TrucksTypeFk)
                            .Include(i => i.ShippingRequests)
                             .ThenInclude(r => r.ShippingRequests)
                              .ThenInclude(r => r.ShippingRequestVases)
                               .ThenInclude(r => r.VasFk)
                            .FirstOrDefaultAsync(i => i.Id == GroupId);
        }
        #endregion
    }
}
