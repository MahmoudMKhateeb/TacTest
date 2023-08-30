using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TACHYON.Reports.ReportParameterDefinitions
{
    public interface IReportParameterDefinitionManager : IDomainService
    {
        Task<List<ReportParameterDefinitionItem>> GetParameterDefinition(int reportDefinitionId);

        Task<List<ReportParameterDefinitionItem>> GetParameterDefinition(Guid reportId);
    }
}
