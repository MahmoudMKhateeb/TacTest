using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.MultiTenancy;
using TACHYON.Reports.ReportDefinitionPermissions;

namespace TACHYON.Reports.ReportDefinitions
{
    public class ReportDefinitionManager : TACHYONDomainServiceBase, IReportDefinitionManager
    {
        private readonly IRepository<ReportDefinition> _reportDefinitionRepository;
        private readonly IRepository<ReportDefinitionPermission, Guid> _definitionPermissionRepository;
        private readonly TenantManager _tenantManager;

        public ReportDefinitionManager(
            IRepository<ReportDefinition> reportDefinitionRepository,
            IRepository<ReportDefinitionPermission, Guid> definitionPermissionRepository,
            TenantManager tenantManager)
        {
            _reportDefinitionRepository = reportDefinitionRepository;
            _definitionPermissionRepository = definitionPermissionRepository;
            _tenantManager = tenantManager;
        }

        public async Task<List<SelectItemDto>> GetReportDefinitionsByPermission(int? tenantId)
        {
            var editionId = await _tenantManager.Tenants.Where(x => x.Id == tenantId).Select(x => x.EditionId).SingleAsync();
            var grantedDefinitions = await GetGrantedReportDefinitions(tenantId,editionId)
                .Select(definition => new SelectItemDto { DisplayName = definition.DisplayName, Id = definition.Id.ToString() })
                .ToListAsync();
            return grantedDefinitions;
        }

        public async Task<string> GetReportUrl(int reportDefinitionId) 
            => await _reportDefinitionRepository.GetAll().Where(x => x.Id == reportDefinitionId)
                .Select(x => x.ReportTemplate.Url).SingleAsync();

        public async Task<ReportType> GetReportDefinitionType(int reportDefinitionId) 
            => await _reportDefinitionRepository.GetAll().Where(x => x.Id == reportDefinitionId)
                .Select(x => x.Type).SingleAsync();
        public async Task<ReportType> GetReportDefinitionType(Guid templateId) 
            => await _reportDefinitionRepository.GetAll().Where(x => x.ReportTemplateId == templateId)
                .Select(x => x.Type).SingleAsync();

        #region Helpers

        private IQueryable<ReportDefinition> GetGrantedReportDefinitions(int? tenantId, int? editionId)
        {
            DisableTenancyFilters();
            var finalDefinitions = from definition in _reportDefinitionRepository.GetAll()
                from editionPermission in _definitionPermissionRepository.GetAll()
                where definition.Id == editionPermission.ReportDefinitionId
                let isExcluded = _definitionPermissionRepository.GetAll()
                    .Any(x => x.ReportDefinitionId == definition.Id
                              && x.TenantId == tenantId
                              && !x.IsGranted)
                where editionPermission.EditionId == editionId
                      && editionPermission.IsGranted && !isExcluded
                select definition;

            return finalDefinitions;
        }


        #endregion
        
    }
}