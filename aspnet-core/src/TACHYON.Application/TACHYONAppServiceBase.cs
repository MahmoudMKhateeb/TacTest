using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.IdentityFramework;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using Abp.Threading;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using TACHYON.Authorization.Users;
using TACHYON.MultiTenancy;
using System.Globalization;
using System.Linq;
using Abp.UI;
using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TACHYON.Configuration;

namespace TACHYON
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class TACHYONAppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        public string CurrentLanguage { get; set; }

        protected IConfigurationProvider AutoMapperConfigurationProvider { get; set; }

        protected int TachyonEditionId => Convert.ToInt32(SettingManager.GetSettingValue(AppSettings.Editions.TachyonEditionId));
        protected int ShipperEditionId => Convert.ToInt32(SettingManager.GetSettingValue(AppSettings.Editions.ShipperEditionId));
        protected int CarrierEditionId => Convert.ToInt32(SettingManager.GetSettingValue(AppSettings.Editions.CarrierEditionId));

        protected TACHYONAppServiceBase()
        {
            LocalizationSourceName = TACHYONConsts.LocalizationSourceName;
            CurrentLanguage = CultureInfo.CurrentCulture.Name;
            var mapper = IocManager.Instance.Resolve<IMapper>();
            AutoMapperConfigurationProvider = mapper.ConfigurationProvider;
        }

        protected virtual async Task<User> GetCurrentUserAsync()
        {
            var user = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            return user;
        }

        protected virtual User GetCurrentUser()
        {
            return AsyncHelper.RunSync(GetCurrentUserAsync);
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
            }
        }

        protected virtual Tenant GetCurrentTenant()
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                return TenantManager.GetById(AbpSession.GetTenantId());
            }
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        protected virtual void DisableTenancyFiltersIfHost()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant);
            }
        }

        protected virtual void DisableTenancyFilters()
        {
            CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant);

        }

        /// <summary>
        /// Because host need to access the service by services
        /// </summary>
        /// <param name="IsHostEnabled"></param>
        /// <param name="features"></param>
        protected void CheckIfCanAccessService(bool IsHostEnabled, params string[] features)
        {
            if ((IsHostEnabled && !AbpSession.TenantId.HasValue) || features.Any(feature => IsEnabled(feature))) return;
            features.Any(feature => IsEnabled(feature));

            throw new UserFriendlyException("YouDoNotHavePermissionToAccessThePage");
        }


        public async Task<LoadResult> LoadResultAsync<T>(IQueryable<T> query, string filter)
        {
            DataSourceLoadOptionsBase dataSourceLoadOptionsBase = JsonConvert.DeserializeObject<DataSourceLoadOptionsBase>(filter);
            return await DataSourceLoader.LoadAsync(query, dataSourceLoadOptionsBase);
        }





        /// <summary>
        /// Take all data with 0 Skip 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static async Task<PagedResultDto<T>> LoadResultWithoutPagingAsync<T>(IQueryable<T> query, string filter)
        {
            DataSourceLoadOptionsBase dataSourceLoadOptionsBase =
                JsonConvert.DeserializeObject<DataSourceLoadOptionsBase>(filter);

            dataSourceLoadOptionsBase.Skip = 0;
            dataSourceLoadOptionsBase.Take = Int32.MaxValue;
            LoadResult loadResult = await DataSourceLoader.LoadAsync(query, dataSourceLoadOptionsBase);
            return new PagedResultDto<T>(loadResult.totalCount, (IReadOnlyList<T>)loadResult.data);
        }



        public class TachyonLoadResult<T> : LoadResult
        {
            public new IEnumerable<T> data { get; set; }


        }
    }
}