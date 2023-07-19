using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading;
using DevExpress.DataAccess.Json;
using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Web;
using TACHYON.Reports.ReportTemplates;

namespace TACHYON.Web.Services.StorageServices
{
    public class CustomReportProvider : IReportProvider
    {
        private readonly IReportTemplateManager _reportTemplateManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public CustomReportProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _unitOfWorkManager = IocManager.Instance.Resolve<IUnitOfWorkManager>();
            _reportTemplateManager = IocManager.Instance.Resolve<IReportTemplateManager>();
        }
        
        public XtraReport GetReport(string id, ReportProviderContext context)
        {
            // Parse the string with the report name and parameter values.
            string[] parts = id.Split('?');
            string reportUrl = parts[0];
            string parametersString = parts.Length > 1 ? parts[1] : string.Empty;
            var parameters = HttpUtility.ParseQueryString(parametersString);
            // Create a report instance.
            XtraReport report = new XtraReport();
            
            var reportTemplate = AsyncHelper.RunSync(() => _reportTemplateManager.GetReportTemplateByUrl(reportUrl));
            using var reportTemplateStream = new MemoryStream(reportTemplate.Data);
            report.LoadLayoutFromXml(reportTemplateStream);
            report.Parameters["ReportId"].Value = parameters["reportId"];

            if (!(report.DataSource is JsonDataSource { JsonSource: UriJsonSource jsonSource })) return report;

            jsonSource.HeaderParameters.Add(new HeaderParameter("Authorization",GetHeaderValue("Authorization")));
            jsonSource.QueryParameters.Add(new QueryParameter("ReportId",typeof(string),parameters["reportId"]));
            using var uow = _unitOfWorkManager.Begin();
            AsyncHelper.RunSync(() => _reportTemplateManager.UpdateReportTemplate(reportUrl, report));
            uow.Complete();
            return report;
        }
        private string GetHeaderValue(string headerName)
        {
            string headerValue = _contextAccessor.HttpContext.Request.Headers[headerName].ToString();
            return headerValue;
        }

    }
}
