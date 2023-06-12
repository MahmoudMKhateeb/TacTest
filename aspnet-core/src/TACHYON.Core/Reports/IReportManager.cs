using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Reports.ReportParameters;

namespace TACHYON.Reports
{
    public interface IReportManager : IDomainService
    {
        Task CreateReport(Report createdReport, List<int> inputGrantedRoles, List<long> inputExcludedUsers, List<ReportParameterDto> inputParameters);
    }
}