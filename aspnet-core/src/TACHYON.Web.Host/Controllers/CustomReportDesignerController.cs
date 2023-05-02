using Abp.Dependency;
using DevExpress.AspNetCore.Reporting.QueryBuilder;
using DevExpress.AspNetCore.Reporting.ReportDesigner;
using DevExpress.AspNetCore.Reporting.ReportDesigner.Native.Services;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.Web.ReportDesigner;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TACHYON.Web.Controllers
{
  // for more information about this setting please see https://supportcenter.devexpress.com/Ticket/Details/T648558/
  
    [ApiExplorerSettings(IgnoreApi = true)]  
    [Route("DXXRD")] 
    public class CustomReportDesignerController : ReportDesignerController, ITransientDependency
    {
        private const string TripsTable = "ShippingRequestTrips";
        private const string DatabaseConnection = "Default";
        private const string DefaultDatasourceName = "TACHYON";
        
        public CustomReportDesignerController(IReportDesignerMvcControllerService controllerService) : base(controllerService)
        {
        }
        
        [HttpPost("[action]")]
        public IActionResult GetDesignerModel([FromForm]string reportUrl, [FromServices] IReportDesignerClientSideModelGenerator modelGenerator)
        {
            // var designerDataSources = new Dictionary<string, object>();
            // var preconfiguredDataSource = new SqlDataSource(DatabaseConnection); // see appsettings.json
            //
            // // Create a Sql query to access the `ShippingRequestTrips` Table
            // // I will provide a trips table as default datasource when he use Report Designer for the first time.
            // SelectQuery tripsQuery = SelectQueryFluentBuilder.AddTable(TripsTable).SelectAllColumns().Build(TripsTable);
            //
            // preconfiguredDataSource.Queries.Add(tripsQuery);
            // preconfiguredDataSource.RebuildResultSchema();
            // designerDataSources.Add(DefaultDatasourceName,preconfiguredDataSource);
            
            var model = modelGenerator.GetModel(reportUrl, null, ReportDesignerController.DefaultUri, WebDocumentViewerController.DefaultUri, QueryBuilderController.DefaultUri);
            return DesignerModel(model);
        }
    }
}