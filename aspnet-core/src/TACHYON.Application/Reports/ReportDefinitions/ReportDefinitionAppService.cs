using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Validation;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Common;
using TACHYON.DataFilters;
using TACHYON.Dto;
using TACHYON.MultiTenancy;
using TACHYON.ReportParameterDefinitions;
using TACHYON.Reports.ReportDefinitions.Dto;
using TACHYON.Reports.ReportParameterDefinitions;
using TACHYON.Reports.ReportTemplates;
using TACHYON.Reports.ReportTemplates.Dto;

namespace TACHYON.Reports.ReportDefinitions
{
    [AbpAuthorize(AppPermissions.Pages_ReportDefinitions)]
    public class ReportDefinitionAppService : TACHYONAppServiceBase
    {
        private readonly IRepository<ReportDefinition> _reportDefinitionRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IReportTemplateManager _reportTemplateManager;
        private readonly IReportParameterDefinitionProvider _definitionProvider;

        public ReportDefinitionAppService(
            IRepository<ReportDefinition> reportDefinitionRepository,
            IRepository<Tenant> tenantRepository,
            IReportTemplateManager reportTemplateManager, 
            IReportParameterDefinitionProvider definitionProvider)
        {
            _reportDefinitionRepository = reportDefinitionRepository;
            _tenantRepository = tenantRepository;
            _reportTemplateManager = reportTemplateManager;
            _definitionProvider = definitionProvider;
        }

        public async Task<LoadResult> GetAll(LoadOptionsInput input)
        {
            DisableTenancyFilters();
            var reportDefinitions = GetReportDefinitionsWithoutActiveFilter()
                .ProjectTo<ReportDefinitionListItemDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(reportDefinitions, input.LoadOptions);
        }
        public async Task CreateOrEdit(CreateOrEditReportDefinitionDto input)
        {
            if (input.Id.HasValue)
            {
                throw new UserFriendlyException(L("UpdateReportTypeNotSupported"));
            }

            await Create(input);
        }

        [AbpAuthorize(AppPermissions.Pages_ReportDefinitions_Create)]
        protected virtual async Task Create(CreateOrEditReportDefinitionDto input)
        {
            await ValidateReportTypeInput(input);
            
            var createdReportDefinition = ObjectMapper.Map<ReportDefinition>(input);

            createdReportDefinition.ReportTemplateId =
              await _reportTemplateManager.GetReportTemplateIdByUrl(input.ReportTemplateUrl);

            // todo move this logic to a Manager 
            var reportParameterDefinitions =
                _definitionProvider.GetParameterDefinitions(createdReportDefinition.Type);

            bool anySelectedParameterDefinitionNotRegistered = input.ParameterDefinitions.Any(selectedParameterDefinition =>
                reportParameterDefinitions.All(predefinedParameterDefinition =>
                    !predefinedParameterDefinition.Name.Equals(selectedParameterDefinition)));

            if (anySelectedParameterDefinitionNotRegistered)
                throw new AbpValidationException(L("NotSupportedFilterIsSelected"));

            createdReportDefinition.ParameterDefinitions = reportParameterDefinitions
                .Where(x => input.ParameterDefinitions.Contains(x.Name))
                .Select(x => new ReportParameterDefinition { Name = x.Name }).ToList();

            await _reportDefinitionRepository.InsertAsync(createdReportDefinition);
        }


        [AbpAuthorize(AppPermissions.Pages_ReportDefinitions_Clone)]
        public async Task<ClonedReportDefinitionDto> GetReportDefinitionForClone(int reportDefinitionId)
        {
            DisableTenancyFilters();
            var clonedReportDefinition = await GetReportDefinitionsWithoutActiveFilter()
                .Include(x=> x.ParameterDefinitions).Include(x=> x.DefinitionPermissions)
                .SingleAsync(x => x.Id == reportDefinitionId);

            return ObjectMapper.Map<ClonedReportDefinitionDto>(clonedReportDefinition);
        }
        public List<ReportParameterDefinitionItemDto> GetReportFilters(ReportType reportType)
        {
            var reportFiltersList = _definitionProvider.GetParameterDefinitions(reportType);
            return ObjectMapper.Map<List<ReportParameterDefinitionItemDto>>(reportFiltersList);
        }

        /// <summary>
        /// Create a template for a new report definition, this End-Point usually used in create report definition/type wizard 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="reportName"></param>
        /// <returns></returns>
         [AbpAuthorize(AppPermissions.Pages_ReportDefinitions_Create)]
        public async Task<ReportTemplateUrlDto> CreateTemplateByName(CreateReportTemplateByNameInput input)
        {
            string createdReportTemplateUrl = await _reportTemplateManager.CreateReportTemplate(input.ReportDefinitionType, input.ReportDefinitionName);
            return new ReportTemplateUrlDto { Url = createdReportTemplateUrl };
        }

        [AbpAuthorize(AppPermissions.Pages_ReportDefinitions_Create)]
        public async Task<bool> IsReportDefinitionNameUsedBefore(string definitionName)
        {
            bool isNameUsedBefore = await _reportDefinitionRepository.GetAll()
                .AnyAsync(x => x.DisplayName.Trim().ToLower().Equals(definitionName.Trim().ToLower()));
            
            bool isDuplicatedUrl = await _reportTemplateManager.IsUrlDuplicated(definitionName.Trim());

            return isNameUsedBefore || isDuplicatedUrl;
        }
        [AbpAuthorize(AppPermissions.Pages_ReportDefinitions_Activate)]
        public async Task Activate(int reportDefinitionId)
        => await ChangeActiveStatus(reportDefinitionId, true);
        [AbpAuthorize(AppPermissions.Pages_ReportDefinitions_Deactivate)]
        public async Task Deactivate(int reportDefinitionId)
        => await ChangeActiveStatus(reportDefinitionId, false);

        #region Drop-Downs
        public async Task<List<SelectItemDto>> GetCompanies(params int?[] tenantIds)
        {
            return await _tenantRepository.GetAll().Where(x => tenantIds.Contains(x.EditionId)).Select(x =>
                new SelectItemDto { Id = x.Id.ToString(), DisplayName = x.Name }).ToListAsync();
        }
        #endregion
        

        #region Helpers
        protected virtual async Task ChangeActiveStatus(int id, bool isActive)
        {
            var reportDefinition = await GetReportDefinitionsWithoutActiveFilter().Where(x => x.Id == id)
                .Select(x => new { x.Id, x.IsActive }).FirstOrDefaultAsync();
            
            switch (reportDefinition)
            {
                case null:
                    throw new UserFriendlyException(L("ReportTypeNotFound"));
                case {IsActive: {} currentStatus} when currentStatus == isActive:
                    throw new UserFriendlyException(L(isActive ? "AlreadyActivated" : "AlreadyDeactivated"));
            }
            _reportDefinitionRepository.Update(id, x => x.IsActive = isActive);
        }
        
        protected virtual async Task ValidateReportTypeInput(CreateOrEditReportDefinitionDto input)
        {

            bool isNameAlreadyUsed = await _reportDefinitionRepository.GetAll().AnyAsync(x =>
                x.DisplayName.Trim().ToLower().Equals(input.DisplayName.Trim().ToLower()));

            if (isNameAlreadyUsed) throw new AbpValidationException(L("TheReportTypeNameAlreadyUsedBefore"));
        }

        protected virtual IQueryable<ReportDefinition> GetReportDefinitionsWithoutActiveFilter()
        {
            CurrentUnitOfWork.DisableFilter(TACHYONDataFilters.ActiveReportDefinition);
            return _reportDefinitionRepository.GetAll();
        }
        #endregion
        
    }
}
