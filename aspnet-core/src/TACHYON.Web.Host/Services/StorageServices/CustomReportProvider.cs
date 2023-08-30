using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Threading;
using Abp.UI;
using DevExpress.DataAccess.Json;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Web;
using TACHYON.Reports;
using TACHYON.Reports.ReportDefinitions;
using TACHYON.Reports.ReportTemplates;
using TACHYON.Web.Services.DataSourceServices;

namespace TACHYON.Web.Services.StorageServices
{
    public class CustomReportProvider : IReportProvider
    {
        private readonly IReportTemplateManager _reportTemplateManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IReportDefinitionManager _definitionManager;
        private readonly CustomDataSourceWizardJsonDataConnectionStorage _connectionStorage;

        public CustomReportProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _connectionStorage = new CustomDataSourceWizardJsonDataConnectionStorage(contextAccessor);
            _unitOfWorkManager = IocManager.Instance.Resolve<IUnitOfWorkManager>();
            _reportTemplateManager = IocManager.Instance.Resolve<IReportTemplateManager>();
            _definitionManager = IocManager.Instance.Resolve<IReportDefinitionManager>();
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

            
            if (report.DataSource == null || report.DataSource is JsonDataSource { JsonSource: null })
            {
                ReportType type = AsyncHelper.RunSync(() => _definitionManager.GetReportDefinitionType(reportTemplate.Id));
                var dataSource = new JsonDataSource
                {
                    JsonSource = _connectionStorage.CreateDefaultDataSourceByReportType(type)
                };
                DataSourceManager.AddDataSources(report,dataSource);
            }

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
