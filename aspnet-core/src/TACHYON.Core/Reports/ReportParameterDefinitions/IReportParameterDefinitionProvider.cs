using Abp.Dependency;
using Abp.Domain.Services;
using System.Collections.Generic;

namespace TACHYON.Reports.ReportParameterDefinitions
{
    public interface IReportParameterDefinitionProvider : IDomainService, ISingletonDependency
    {
        IEnumerable<StaticReportParameterDefinition> GetParameterDefinitions(ReportType type);

        bool IsParameterDefined(string parameterName, ReportType reportType);

        bool AnyParameterNotDefined(ReportType reportType, params string[] parameterNames);

        StaticReportParameterDefinition GetParameterDefinition(ReportType type, string parameterName);
    }
}