using Abp.Domain.Services;
using System;
using System.Threading.Tasks;
using Task = Twilio.TwiML.Voice.Task;

namespace TACHYON.Reports.ReportTemplates
{
    public interface IReportTemplateManager : IDomainService
    {
        Task<string> CreateReportTemplate(ReportType type, string reportDefinitionName);
        Task<Guid> GetReportTemplateIdByUrl(string url);
        
        Task<ReportTemplate> GetReportTemplateByUrl(string url);
    }
}