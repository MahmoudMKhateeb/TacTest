using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Dto;

namespace TACHYON.Reports.ReportDefinitions
{
    public interface IReportDefinitionManager : IDomainService
    {
        Task<List<SelectItemDto>> GetReportDefinitionsByPermission(int? tenantId);
        Task<string> GetReportUrl(int reportDefinitionId);

        Task<ReportType> GetReportDefinitionType(int reportDefinitionId);

        Task<ReportType> GetReportDefinitionType(Guid templateId);
    }
}