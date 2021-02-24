using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
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
using TACHYON.Invoices.Balances;
using TACHYON.Invoices.Groups.Dto;
using TACHYON.Invoices.Periods;
using TACHYON.Notifications;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Storage;

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
        private readonly BalanceManager _BalanceManager;
        private readonly IBinaryObjectManager _BinaryObjectManager;
        private readonly IAppNotifier _appNotifier;
        private readonly InvoiceManager _invoiceManager;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        public GroupPeriodAppService(
            IRepository<GroupPeriod, long> repository, 
            BalanceManager BalanceManager, 
            IRepository<InvoicePeriod> PeriodRepository,
            IRepository<GroupPeriodInvoice, long> GroupPeriodInvoiceRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            CommonManager commonManager,
            UserManager userManager,
            IBinaryObjectManager BinaryObjectManager,
            IAppNotifier appNotifier,
            InvoiceManager invoiceManager,
            ITempFileCacheManager tempFileCacheManager,
            IBinaryObjectManager binaryObjectManager)
        {
            _Repository = repository;
            _PeriodRepository = PeriodRepository;
            _GroupPeriodInvoiceRepository = GroupPeriodInvoiceRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _BalanceManager = BalanceManager;
            _commonManager = commonManager;
            _userManager = userManager;
            _BinaryObjectManager = BinaryObjectManager;
            _appNotifier = appNotifier;
            _invoiceManager = invoiceManager;
            _tempFileCacheManager = tempFileCacheManager;
            _binaryObjectManager = binaryObjectManager;
        }


        [AbpAuthorize(AppPermissions.Pages_Invoices_GroupsPeriods)]
        public async Task<PagedResultDto<GroupPeriodListDto>> GetAll(GroupPeriodFilterInput input)
        {
            IQueryable<GroupPeriod> query= _commonManager.ExecuteMethodIfHostOrTenantUsers(() => GetGroupPeriods(input));

            var totalCount = await query.CountAsync();

            return new PagedResultDto<GroupPeriodListDto>(
                totalCount,
                ObjectMapper.Map<List<GroupPeriodListDto>>(query)
            );

        }

        [AbpAuthorize(AppPermissions.Pages_Invoices_GroupsPeriods)]

        public async Task<GroupPeriodInfoDto> GetById(EntityDto input)
        {
            var Group = await _commonManager.ExecuteMethodIfHostOrTenantUsers(() => GetGroupPeriodInfo(input.Id));

            var GroupDto = ObjectMapper.Map<GroupPeriodInfoDto>(Group);
            if (GroupDto != null)
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                 
                    var user = await _userManager.Users.SingleAsync(u => u.TenantId == Group.TenantId && u.UserName== AbpUserBase.AdminUserName);

                    GroupDto.Email = user.EmailAddress;
                    GroupDto.Phone = user.PhoneNumber;
                }

            }
            return GroupDto;

        }

        private IQueryable<GroupPeriod> GetGroupPeriods(GroupPeriodFilterInput input)
        {
            var query = _Repository
                .GetAll()
                .Include(i => i.Tenant)
                .Include(i => i.InvoicePeriod)
                .WhereIf(input.TenantId.HasValue, i => i.TenantId == input.TenantId)
                .WhereIf(input.IsDemand.HasValue, i => i.IsDemand == input.IsDemand)
                 .WhereIf(input.PeriodId.HasValue, i => i.PeriodId == input.PeriodId)
                .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue, i => i.CreationTime >= input.FromDate && i.CreationTime < input.ToDate)
                .OrderBy(input.Sorting ?? "IsDemand asc")
                .PageBy(input);
            return query;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_GroupsPeriods_Delete)]

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
            if (Group.IsDemand)
            {
                await _BinaryObjectManager.DeleteAsync((Guid)Group.BinaryObjectId);
            }
            if (Group.IsClaim)
            {
                var Invoice = await _GroupPeriodInvoiceRepository.SingleAsync(g => g.GroupId == Group.Id);
                await _invoiceManager.RemoveInvoiceFromRequest(Invoice.InvoiceId);
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Invoices_GroupsPeriods_Demand)]
        public  async Task Demand(GroupPeriodDemandCreateInput Input)
        {
            var Group = await GetGroupPeriod(Input.Id);
            if (Group.IsDemand) return;
            var fileBytes = Convert.FromBase64String(Input.DocumentBase64.Split(',')[1]);
            if (fileBytes.Length > 1048576 * 100) //100 MB
            {
                throw new UserFriendlyException(L("File_SizeLimit_Error"));
            }

            var fileObject = new BinaryObject(Group.TenantId, fileBytes);
            await _BinaryObjectManager.SaveAsync(fileObject);
            Group.IsDemand = true;
            Group.BinaryObjectId = fileObject.Id;
            Group.DemandFileContentType = Input.ContentType;
            Group.DemandFileName = Input.FileName;
            await  _appNotifier.GroupPeriodOnDemand(Group);
        }

        [AbpAuthorize(AppPermissions.Pages_Invoices_GroupsPeriods_UnDemand)]
        public async Task UnDemand(long GroupId)
        {
            var Group = await GetGroupPeriod(GroupId);
            if (Group.IsDemand && !Group.IsClaim)
            {
                Group.IsDemand = false;
                Group.DemandFileContentType = null;
                Group.DemandFileName = null;
                Group.BinaryObjectId = default(Guid);
               await _BinaryObjectManager.DeleteAsync((Guid)Group.BinaryObjectId);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_GroupsPeriods_Claim)]
        public async Task Claim(long GroupId)
        {
            var Group = await _Repository.GetAllIncluding(g => g.ShippingRequests, g => g.Tenant).
                SingleAsync(g => g.Id == GroupId && g.IsDemand==true && g.IsClaim==false);            
            if (Group !=null)
            {
               
               await _invoiceManager.GenerateCarrirInvoice(Group);
               Group.IsClaim = true;
            }
        }


        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_GroupsPeriods_UnClaim)]
        public async Task UnClaim(long GroupId)
        {
            var Group = await _Repository.GetAllIncluding(g => g.ShippingRequests, g => g.Tenant).
                SingleAsync(g => g.Id == GroupId && g.IsClaim == true);

            if (Group != null)
            {
                var Invoice = await _GroupPeriodInvoiceRepository.SingleAsync(g => g.GroupId == Group.Id);

                await _invoiceManager.RemoveInvoiceFromRequest(Invoice.InvoiceId);
                await _GroupPeriodInvoiceRepository.DeleteAsync(Invoice);
                Group.IsClaim = false;

            }
        }


        [AbpAuthorize(AppPermissions.Pages_Invoices_GroupsPeriods)]
        public async Task<FileDto> GetFileDto(long GroupId)

        {
            DisableTenancyFiltersIfHost();
            var documentFile = await _Repository.SingleAsync(g => g.Id == GroupId && g.IsDemand);
            if (documentFile == null)
            {
                throw new UserFriendlyException(L("TheRequestNotFound"));

            }


           var binaryObject = await _binaryObjectManager.GetOrNullAsync(documentFile.BinaryObjectId.Value);

            var file = new FileDto(documentFile.DemandFileName, documentFile.DemandFileContentType);

            _tempFileCacheManager.SetFile(file.FileToken, binaryObject.Bytes);

            return file;
        }


        #region Heleper
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
                              .ThenInclude(r => r.TrucksTypeFk)
                            .SingleAsync(i => i.Id == GroupId);
        }
        #endregion
    }
}
