using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Threading;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Reports.ReportDefinitions;

namespace TACHYON.Reports.ReportParameterDefinitions
{
    public class ReportParameterDefinitionManager : TACHYONDomainServiceBase, IReportParameterDefinitionManager
    {

        private readonly IRepository<ReportParameterDefinition, Guid> _parameterDefinitionRepository;
        private readonly IRepository<Report, Guid> _reportRepository;
        private readonly IRepository<ReportDefinition> _reportDefinitionRepository;
        private readonly IReportParameterDefinitionProvider _definitionProvider;

        public ReportParameterDefinitionManager(
            IRepository<ReportParameterDefinition, Guid> parameterDefinitionRepository,
            IRepository<Report, Guid> reportRepository, 
            IRepository<ReportDefinition> reportDefinitionRepository,
            IReportParameterDefinitionProvider definitionProvider)
        {
            _parameterDefinitionRepository = parameterDefinitionRepository;
            _reportRepository = reportRepository;
            _reportDefinitionRepository = reportDefinitionRepository;
            _definitionProvider = definitionProvider;
        }

        public async Task<List<ReportParameterDefinitionItem>> GetParameterDefinition(int reportDefinitionId)
        {
            var reportDefinitionType = await _reportDefinitionRepository.GetAll().Where(x => x.Id == reportDefinitionId)
                .Select(x => x.Type).SingleAsync();

            return await GetReportParameterDefinitions(reportDefinitionId, reportDefinitionType);
        }
        
        public async Task<List<ReportParameterDefinitionItem>> GetParameterDefinition(Guid reportId)
        {
            var reportDefinition = await _reportRepository.GetAll().Where(x => x.Id.Equals(reportId))
                .Select(x => new { Id = x.ReportDefinitionId, x.ReportDefinition.Type }).SingleAsync();

            return await GetReportParameterDefinitions(reportDefinition.Id, reportDefinition.Type);
        }

        #region Helpers

        private async Task<List<ReportParameterDefinitionItem>> GetReportParameterDefinitions(int reportDefinitionId, ReportType reportDefinitionType)
        {
            var selectedParameterNames = await _parameterDefinitionRepository.GetAll()
                .Where(x => x.ReportDefinitionId == reportDefinitionId)
                .Select(x => x.Name).ToListAsync();

            var parameterDefinitions = _definitionProvider.GetParameterDefinitions(reportDefinitionType);

            return parameterDefinitions.Where(x => selectedParameterNames.Contains(x.Name)).Select((x) =>
                new ReportParameterDefinitionItem
                {
                    DisplayName = L(x.Name),
                    ParameterName = x.Name,
                    ParameterType = GetParameterType(x),
                    ListData = x.ListDataCallback != null ? AsyncHelper.RunSync(x.ListDataCallback) : null
                }).ToList();
        }

        private ReportParameterType GetParameterType(StaticReportParameterDefinition parameterDefinition)
        {
            if (parameterDefinition.ListDataCallback is not null)
                return ReportParameterType.List;
            if (parameterDefinition.Type == typeof(bool))
                return ReportParameterType.Boolean;
            if (IsNumberType(parameterDefinition.Type))
                return ReportParameterType.Number;
            if (parameterDefinition.Type == typeof(string))
                return ReportParameterType.String;
            if (typeof(IEnumerable).IsAssignableFrom(parameterDefinition.Type))
                return ReportParameterType.List;
            if (parameterDefinition.Type == typeof(DateTime))
                return ReportParameterType.DateTime;

            return ReportParameterType.Unknown;
        }

        private bool IsNumberType(Type type)
        {
            return type.IsPrimitive && ((type != typeof(bool) && type != typeof(char)) ||
                   type == typeof(decimal));
        }

        #endregion
        
    }
}
