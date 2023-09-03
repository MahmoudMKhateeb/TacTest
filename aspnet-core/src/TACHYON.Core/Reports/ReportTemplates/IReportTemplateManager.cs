using Abp.Domain.Services;
using DevExpress.XtraReports.UI;
using System;
using System.Threading.Tasks;

namespace TACHYON.Reports.ReportTemplates
{
    public interface IReportTemplateManager : IDomainService
    {
        /// <summary>
        /// this method create a report template with a pre-designed default template
        /// once you initiate a new instance of report (TripDetailsReport, ..etc) you will get a
        /// report with a pre-designed default template
        /// </summary>
        /// <param name="type"></param>
        /// <param name="reportDefinitionName"></param>
        /// <returns>Task(String) </returns>
        Task<string> CreateReportTemplate(ReportType type, string reportDefinitionName);
        Task<Guid> GetReportTemplateIdByUrl(string url);
        
        Task<ReportTemplate> GetReportTemplateByUrl(string url);

        /// <summary>
        /// this method used to update report data (report .repx file)...
        /// usually used after update report parameters or some related data
        /// </summary>
        /// <param name="url"></param>
        /// <param name="report"></param>
        /// <returns>Task</returns>
        Task UpdateReportTemplate(string url, XtraReport report);

        Task<bool> IsUrlDuplicated(string reportDefinitionName);
    }
}   