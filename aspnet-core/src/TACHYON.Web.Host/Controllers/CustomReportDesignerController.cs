
using Abp.Dependency;
using Abp.Web.Models;
using DevExpress.AspNetCore.Reporting.QueryBuilder;
using DevExpress.AspNetCore.Reporting.ReportDesigner;
using DevExpress.AspNetCore.Reporting.ReportDesigner.Native.Services;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Json;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.ReportDesigner;
using DevExpress.XtraReports.Web.ReportDesigner.DataContracts;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;


namespace TACHYON.Web.Controllers
{
  // for more information about this setting please see https://supportcenter.devexpress.com/Ticket/Details/T648558/

  [ApiExplorerSettings(IgnoreApi = true)]
  [Route("DXXRD")]
  [DontWrapResult]
    public class CustomReportDesignerController : ReportDesignerController, ITransientDependency
  {
      private const string TripsTable = "ShippingRequestTrips";
      private const string DatabaseConnection = "Default";
      private const string DefaultDatasourceName = "TACHYON";

      public CustomReportDesignerController(IReportDesignerMvcControllerService controllerService) : base(
          controllerService)
      {
      }

      [HttpPost("[action]")]
      
      public IActionResult GetDesignerModel([FromForm] string reportUrl,
          [FromServices] IReportDesignerClientSideModelGenerator modelGenerator)
      {
            var designerDataSources = new Dictionary<string, object>();

            var model = modelGenerator.GetModel(reportUrl, designerDataSources, DefaultUri, WebDocumentViewerController.DefaultUri,
              QueryBuilderController.DefaultUri);

          
            model.WizardSettings = new WizardSettings()
            {
                EnableSqlDataSource = true,
                EnableJsonDataSource = true,
                EnableObjectDataSource = true,
                UseFullscreenWizard = true,
                UseMasterDetailWizard = true,
            };
           
          return DesignerModel(model);
      }

  }
}