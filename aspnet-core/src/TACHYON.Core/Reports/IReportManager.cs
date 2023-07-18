using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Reports.ReportParameters;

namespace TACHYON.Reports
{
    public interface IReportManager : IDomainService
    {
        IQueryable<Report> GetAll(long userId);
        Task<Guid> CreateReport(Report createdReport, List<int> inputGrantedRoles, List<long> inputExcludedUsers, List<ReportParameterDto> inputParameters);

        Task GenerateReportFile(Guid reportId);
        Task Publish(Guid reportId);

        Task Delete(Guid reportId);
    }
}