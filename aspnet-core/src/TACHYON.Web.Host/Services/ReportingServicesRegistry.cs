using DevExpress.AspNetCore.Reporting;
using DevExpress.XtraReports.Security;
using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.Web.Extensions;
using DevExpress.XtraReports.Web.QueryBuilder.Services;
using DevExpress.XtraReports.Web.ReportDesigner.Services;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using Microsoft.Extensions.DependencyInjection;
using TACHYON.Web.Services.DataSourceServices;
using TACHYON.Web.Services.ExceptionHandlers;
using TACHYON.Web.Services.StorageServices;

namespace TACHYON.Web.Services
{
    public static class ReportingServicesRegistry
    {

        public static void RegisterReportingServices(this IServiceCollection services)
        {
            services.AddScoped<ReportStorageWebExtension, ReportStorageWebExtensionService>();

            services.AddSingleton<IWebDocumentViewerExceptionHandler, CustomWebDocumentViewerExceptionHandler>()
                .AddSingleton<IReportDesignerExceptionHandler, CustomReportDesignerExceptionHandler>()
                .AddSingleton<IQueryBuilderExceptionHandler, CustomQueryBuilderExceptionHandler>();

            services.AddScoped<IReportProvider, CustomReportProvider>();

            ScriptPermissionManager.GlobalInstance = new ScriptPermissionManager(ExecutionMode.Unrestricted);
            services.ConfigureReportingServices(configurator => {
                configurator.ConfigureReportDesigner(designerConfigurator => {

                    designerConfigurator.RegisterDataSourceWizardJsonConnectionStorage<CustomDataSourceWizardJsonDataConnectionStorage>(true);
                });
                configurator.ConfigureWebDocumentViewer(viewerConfigurator => {

                    viewerConfigurator.RegisterJsonDataConnectionProviderFactory<CustomJsonDataConnectionProviderFactory>();
                });
            });
        }

    }
}
