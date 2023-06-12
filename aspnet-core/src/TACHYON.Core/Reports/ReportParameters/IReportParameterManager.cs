using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TACHYON.Reports.ReportParameters
{
    public interface IReportParameterManager : IDomainService
    {
        Task<List<ReportParameterItem>> GetReportParameters(Guid reportId);
        Task<List<ReportParameterItem>> GetReportParameters(string reportUrl);
    }
}