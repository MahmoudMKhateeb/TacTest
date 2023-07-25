using Abp.Authorization;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Roles;
using TACHYON.Common;
using TACHYON.Dto;
using TACHYON.Reports.Dto;
using TACHYON.Reports.ReportDefinitions;
using TACHYON.Reports.ReportParameterDefinitions;

namespace TACHYON.Reports
{

    [AbpAuthorize(AppPermissions.Pages_Reports)]
    public class ReportAppService : TACHYONAppServiceBase
    {
        private readonly RoleManager _roleManager;
        private readonly IReportManager _reportManager;
        private readonly IReportParameterDefinitionManager _parameterDefinitionManager;
        private readonly IReportDefinitionManager _definitionManager;

        public ReportAppService(
            RoleManager roleManager,
            IReportManager reportManager,
            IReportParameterDefinitionManager parameterDefinitionManager,
            IReportDefinitionManager definitionManager)
        {
            _roleManager = roleManager;
            _reportManager = reportManager;
            _parameterDefinitionManager = parameterDefinitionManager;
            _definitionManager = definitionManager;
        }

        public async Task<LoadResult> GetAll(LoadOptionsInput input)
        {
            if (AbpSession.UserId == null)
                throw new AbpAuthorizationException(L("MustBeAuthorized"));

            var reports = _reportManager.GetAll(AbpSession.UserId.Value)
                .ProjectTo<ReportListItemDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(reports, input.LoadOptions);
        }

        public async Task<Guid> CreateOrEdit(CreateOrEditReportDto input)
        {
            if (!input.Id.HasValue)
            {
                return await Create(input);
            }

            return await Update(input);
        }

        [AbpAuthorize(AppPermissions.Pages_Reports_Create)]
        protected virtual async Task<Guid> Create(CreateOrEditReportDto input)
        {
            // add mapping configurations
            var createdReport = ObjectMapper.Map<Report>(input);

            return await _reportManager.CreateReport(createdReport, input.GrantedRoles, input.ExcludedUsers,
                input.Parameters);
        }

        protected virtual async Task<Guid> Update(CreateOrEditReportDto input)
        {
            // note: add permission when implement this action
            throw new UserFriendlyException(L("UpdateReportNotSupportedYet"));
        }

        public async Task<List<ReportParameterDefinitionItem>> GetReportFilters(int reportDefinitionId)
            => await _parameterDefinitionManager.GetParameterDefinition(reportDefinitionId);

        public async Task<string> GetReportUrl(int reportDefinitionId)
            => await _definitionManager.GetReportUrl(reportDefinitionId);

        [AbpAuthorize(AppPermissions.Pages_Reports_Create)]
        public async Task Publish(Guid reportId)
            => await _reportManager.Publish(reportId);

        [AbpAuthorize(AppPermissions.Pages_Reports_Delete)]
        public async Task Delete(Guid reportId)
            => await _reportManager.Delete(reportId);

        #region Drop-Downs

        public async Task<List<SelectItemDto>> GetReportDefinitionsForDropdown()
        {
            return await _definitionManager.GetReportDefinitionsByPermission(AbpSession.TenantId);
        }

        public async Task<List<SelectItemDto>> GetTenantRoles()
        {

            return await _roleManager.Roles
                .Select(x => new SelectItemDto { DisplayName = x.DisplayName, Id = x.Id.ToString() }).ToListAsync();
        }

        public async Task<List<SelectItemDto>> GetTenantUsers(params int[] roleIds)
        {
            return await UserManager.Users.Where(x => x.Roles.Any(r => roleIds.Contains(r.RoleId)) && !x.IsDriver)
                .Select(x => new SelectItemDto { DisplayName = $"{x.Name} {x.Surname}", Id = x.Id.ToString() })
                .ToListAsync();
        }

        #endregion

    }
}