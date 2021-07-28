using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Common.Dto;
using TACHYON.Dto;
using TACHYON.Editions;
using TACHYON.Editions.Dto;
using TACHYON.Features;
using TACHYON.Invoices.Periods;
using TACHYON.MultiTenancy;
using TACHYON.Shipping.Accidents;
using TACHYON.Shipping.Accidents.Dto;

namespace TACHYON.Common
{
    [AbpAuthorize]
    public class CommonLookupAppService : TACHYONAppServiceBase, ICommonLookupAppService
    {
        private readonly EditionManager _editionManager;
        private readonly IRepository<Tenant> _Tenant;
        private readonly IRepository<InvoicePeriod> _PeriodRepository;
        private readonly IRepository<ShippingRequestReasonAccident> _ReasonAccidentRepository;
        private User _CurrentUser;

        public CommonLookupAppService(EditionManager editionManager, IRepository<Tenant> Tenant,
            IRepository<InvoicePeriod> PeriodRepository,
            IRepository<ShippingRequestReasonAccident> ReasonAccidentRepository)
        {
            _editionManager = editionManager;
            _Tenant = Tenant;
            _PeriodRepository = PeriodRepository;
            _ReasonAccidentRepository = ReasonAccidentRepository;
    }

        public async Task<ListResultDto<SubscribableEditionComboboxItemDto>> GetEditionsForCombobox(bool onlyFreeItems = false)
        {
            var subscribableEditions = (await _editionManager.Editions.Cast<SubscribableEdition>().ToListAsync())
                .WhereIf(onlyFreeItems, e => e.IsFree)
                .OrderBy(e => e.MonthlyPrice);

            return new ListResultDto<SubscribableEditionComboboxItemDto>(
                subscribableEditions.Select(e => new SubscribableEditionComboboxItemDto(e.Id.ToString(), e.DisplayName, e.IsFree)).ToList()
            );
        }

        public async Task<PagedResultDto<NameValueDto>> FindUsers(FindUsersInput input)
        {
            if (AbpSession.TenantId != null)
            {
                //Prevent tenants to get other tenant's users.
                input.TenantId = AbpSession.TenantId;
            }

            using (CurrentUnitOfWork.SetTenantId(input.TenantId))
            {
                var query = UserManager.Users
                    .WhereIf(
                        !input.Filter.IsNullOrWhiteSpace(),
                        u =>
                            u.Name.Contains(input.Filter) ||
                            u.Surname.Contains(input.Filter) ||
                            u.UserName.Contains(input.Filter) ||
                            u.EmailAddress.Contains(input.Filter)
                    ).WhereIf(input.ExcludeCurrentUser, u => u.Id != AbpSession.GetUserId())
                    .WhereIf(input.ExcludeDrivers, u => !u.IsDriver );

                var userCount = await query.CountAsync();
                var users = await query
                    .OrderBy(u => u.Name)
                    .ThenBy(u => u.Surname)
                    .PageBy(input)
                    .ToListAsync();

                return new PagedResultDto<NameValueDto>(
                    userCount,
                    users.Select(u =>
                        new NameValueDto(
                            u.FullName + " (" + u.EmailAddress + ")",
                            u.Id.ToString()
                            )
                        ).ToList()
                    );
            }
        }

        public GetDefaultEditionNameOutput GetDefaultEditionName()
        {
            return new GetDefaultEditionNameOutput
            {
                Name = EditionManager.DefaultEditionName
            };
        }

        public IEnumerable<ISelectItemDto> GetAutoCompleteTenants(string name,string EdtionName)
        {
            name = name.ToLower().Trim();
            var query =
                _Tenant.GetAll().              
                Where(t => t.IsActive && (t.Name.ToLower().Contains(name) || t.TenancyName.ToLower().Contains(name)) && (string.IsNullOrEmpty(EdtionName) || t.Edition.DisplayName.ToLower().Contains(EdtionName.ToLower().Trim())))
                .Select(t => new SelectItemDto { DisplayName = t.Name, Id = t.Id.ToString() }).Take(20).ToList();

            return query;
        }


        public async Task<List<SelectItemDto>> GetPeriods()
        {
            _CurrentUser = GetCurrentUser();

            var query = await _PeriodRepository.GetAll()
                .WhereIf(_CurrentUser.TenantId.HasValue && FeatureChecker.IsEnabled(AppFeatures.Carrier),p=> p.ShipperOnlyUsed==false)
                .Where(p => p.Enabled == true)
                .Select(t => new SelectItemDto { DisplayName = t.DisplayName, Id = t.Id.ToString() }).ToListAsync();
            return query;
        }

        public  Task<List<SelectItemDto>> GetAccidentReason()
        {
            var query=  _ReasonAccidentRepository
                .GetAllIncluding(x => x.Translations)
                .AsNoTracking();
            return Task.FromResult(ObjectMapper.Map<List<ShippingRequestReasonAccidentListDto>>(query).Select(t => new SelectItemDto { DisplayName = t.Name, Id = t.Id.ToString() }).ToList());
            
        }
    }
}