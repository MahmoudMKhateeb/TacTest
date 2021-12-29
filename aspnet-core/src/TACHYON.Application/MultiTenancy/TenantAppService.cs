using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Security;
using Abp.Timing;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users.Dto;
using TACHYON.Cities;
using TACHYON.Cities.Dtos;
using TACHYON.Countries;
using TACHYON.Dto;
using TACHYON.Editions.Dto;
using TACHYON.MultiTenancy.Dto;
using TACHYON.Url;

namespace TACHYON.MultiTenancy
{
    [AbpAuthorize(AppPermissions.Pages_Tenants)]
    public class TenantAppService : TACHYONAppServiceBase, ITenantAppService
    {
        public IAppUrlService AppUrlService { get; set; }
        public IEventBus EventBus { get; set; }

        private readonly IRepository<County, int> _lookup_countryRepository;
        private readonly IRepository<City, int> _lookup_cityRepository;


        public TenantAppService(IRepository<County, int> lookup_countryRepository, IRepository<City, int> lookup_cityRepository)
        {
            AppUrlService = NullAppUrlService.Instance;
            EventBus = NullEventBus.Instance;
            _lookup_countryRepository = lookup_countryRepository;
            _lookup_cityRepository = lookup_cityRepository;
        }

        public async Task<PagedResultDto<TenantListDto>> GetTenants(GetTenantsInput input)
        {
            var query = TenantManager.Tenants
                .Include(t => t.Edition)
                .WhereIf(!input.Filter.IsNullOrWhiteSpace(), t => t.Name.Contains(input.Filter) || t.TenancyName.Contains(input.Filter))
                .WhereIf(input.CreationDateStart.HasValue, t => t.CreationTime >= input.CreationDateStart.Value)
                .WhereIf(input.CreationDateEnd.HasValue, t => t.CreationTime <= input.CreationDateEnd.Value)
                .WhereIf(input.SubscriptionEndDateStart.HasValue, t => t.SubscriptionEndDateUtc >= input.SubscriptionEndDateStart.Value.ToUniversalTime())
                .WhereIf(input.SubscriptionEndDateEnd.HasValue, t => t.SubscriptionEndDateUtc <= input.SubscriptionEndDateEnd.Value.ToUniversalTime())
                .WhereIf(input.EditionIdSpecified, t => t.EditionId == input.EditionId);

            var tenantCount = await query.CountAsync();
            var tenants = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<TenantListDto>(
                tenantCount,
                ObjectMapper.Map<List<TenantListDto>>(tenants)
                );
        }

        public async Task<LoadResult> GetAllTenants(string loadOptions)
        {
            DisableTenancyFiltersIfHost();

            IQueryable<TenantListDto> tenantListDtos = TenantManager.Tenants.ProjectTo<TenantListDto>(AutoMapperConfigurationProvider);
            IQueryable<UserListDto> userListDtos = UserManager.Users.Where(x => x.UserName.ToLower() == "admin").ProjectTo<UserListDto>(AutoMapperConfigurationProvider);

            var query = from t in tenantListDtos
                        join u in userListDtos
                            on t.Id equals u.TenantId
                        select new GetAllTenantsOutput { TenantListDto = t, UserListDto = u };

            return await LoadResultAsync(query, loadOptions);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_Create)]
        [UnitOfWork(IsDisabled = true)]
        public async Task CreateTenant(CreateTenantInput input)
        {
            var tenancyName = input.companyName.Trim().Replace(" ", "_");

            var tenantId = await TenantManager.CreateWithAdminUserAsync(
                 input.companyName,
                 input.MobileNo,
                 tenancyName,
                 input.Name,
                 input.Address,
                 input.CountryId,
                 input.CityId,
                 input.AdminPassword,
                 input.AdminEmailAddress,
                 input.ConnectionString,
                 input.IsActive,
                 input.EditionId,
                 input.ShouldChangePasswordOnNextLogin,
                 true,
                 input.SubscriptionEndDateUtc?.ToUniversalTime(),
                 input.IsInTrialPeriod,
                 AppUrlService.CreateEmailActivationUrlFormat(tenancyName),
                 input.UserAdminFirstName,
                 input.UserAdminSurname,
                 input.MoiNumber
             );

            var tenant = await TenantManager.GetByIdAsync(tenantId);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_Edit)]
        public async Task<TenantEditDto> GetTenantForEdit(EntityDto input)
        {
            var tenantEditDto = ObjectMapper.Map<TenantEditDto>(await TenantManager.GetByIdAsync(input.Id));
            tenantEditDto.ConnectionString = SimpleStringCipher.Instance.Decrypt(tenantEditDto.ConnectionString);
            return tenantEditDto;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_Edit)]
        public async Task UpdateTenant(TenantEditDto input)
        {

            input.ConnectionString = SimpleStringCipher.Instance.Encrypt(input.ConnectionString);
            var tenant = await TenantManager.GetByIdAsync(input.Id);

            await TenantManager.CheckEditionAsync(tenant.EditionId, input.IsInTrialPeriod);
            // if (tenant.EditionId != input.EditionId)
            // {
            //     EventBus.Trigger(new TenantEditionChangedEventData
            //     {
            //         TenantId = input.Id,
            //         OldEditionId = tenant.EditionId,
            //         NewEditionId = input.EditionId
            //     });
            // }

            ObjectMapper.Map(input, tenant);
            tenant.SubscriptionEndDateUtc = tenant.SubscriptionEndDateUtc?.ToUniversalTime();

            await TenantManager.UpdateAsync(tenant);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_Delete)]
        public async Task DeleteTenant(EntityDto input)
        {
            var tenant = await TenantManager.GetByIdAsync(input.Id);
            await TenantManager.DeleteAsync(tenant);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_ChangeFeatures)]
        public async Task<GetTenantFeaturesEditOutput> GetTenantFeaturesForEdit(EntityDto input)
        {
            var features = FeatureManager.GetAll()
                .Where(f => f.Scope.HasFlag(FeatureScopes.Tenant));
            var featureValues = await TenantManager.GetFeatureValuesAsync(input.Id);

            return new GetTenantFeaturesEditOutput
            {
                Features = ObjectMapper.Map<List<FlatFeatureDto>>(features).OrderBy(f => f.DisplayName).ToList(),
                FeatureValues = featureValues.Select(fv => new NameValueDto(fv)).ToList()
            };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_ChangeFeatures)]
        public async Task UpdateTenantFeatures(UpdateTenantFeaturesInput input)
        {
            await TenantManager.SetFeatureValuesAsync(input.Id, input.FeatureValues.Select(fv => new NameValue(fv.Name, fv.Value)).ToArray());
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_ChangeFeatures)]
        public async Task ResetTenantSpecificFeatures(EntityDto input)
        {
            await TenantManager.ResetAllFeaturesAsync(input.Id);
        }

        public async Task UnlockTenantAdmin(EntityDto input)
        {
            using (CurrentUnitOfWork.SetTenantId(input.Id))
            {
                var tenantAdmin = await UserManager.GetAdminAsync();
                if (tenantAdmin != null)
                {
                    tenantAdmin.Unlock();
                }
            }
        }


        public async Task<List<TenantCountryLookupTableDto>> GetAllCountryForTableDropdown()
        {
            List<County> countries = await _lookup_countryRepository
                .GetAllIncluding(x => x.Translations)
                .OrderBy(x => x.DisplayName)
                .ToListAsync();

            List<TenantCountryLookupTableDto> countryDtos = ObjectMapper.Map<List<TenantCountryLookupTableDto>>(countries);
            return countryDtos;
        }


        public async Task<List<TenantCityLookupTableDto>> GetAllCitiesForTableDropdown(int input)
        {
            List<City> cities = await _lookup_cityRepository
                .GetAllIncluding(x => x.Translations)
                .Where(x => x.CountyFk.Id == input)
                .OrderBy(x => x.DisplayName)
                .ToListAsync();

            List<TenantCityLookupTableDto> cityDtos = ObjectMapper.Map<List<TenantCityLookupTableDto>>(cities);
            return cityDtos;
        }
    }
}