using Abp.Dependency;
using Abp.Threading;
using DevExpress.DataAccess.Json;
using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;
using System;
using System.IO;
using TACHYON.Reports.ReportTemplates;

namespace TACHYON.Web.Services.StorageServices
{
    public class CustomReportProvider : IReportProvider
    {
        private readonly IReportTemplateManager _reportTemplateManager;

        public CustomReportProvider()
        {
            _reportTemplateManager = IocManager.Instance.Resolve<IReportTemplateManager>();
        }
        
        public XtraReport GetReport(string id, ReportProviderContext context)
        {
            // Parse the string with the report name and parameter values.
            string[] parts = id.Split('?');
            string reportUrl = parts[0];
            
            // Create a report instance.
            XtraReport report = new XtraReport();
            
            var reportTemplate = AsyncHelper.RunSync(() => _reportTemplateManager.GetReportTemplateByUrl(reportUrl));
            using var reportTemplateStream = new MemoryStream(reportTemplate.Data);
            report.LoadLayoutFromXml(reportTemplateStream);
            report.Parameters["ReportUrl"].Value = reportUrl;
            if (report.DataSource is JsonDataSource { JsonSource: UriJsonSource jsonSource })
            {
                jsonSource.QueryParameters.Add(new QueryParameter("ReportUrl",typeof(string),reportUrl));
            }
            return report;
        }

    }
}
