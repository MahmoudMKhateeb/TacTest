using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.ReportDesigner.Services;
using System;

namespace TACHYON.Web.Services.ExceptionHandlers
{
    public class CustomReportDesignerExceptionHandler : ReportDesignerExceptionHandler
    {
        public override string GetFaultExceptionMessage(FaultException ex)
        {
            throw ex;
        }

        public override string GetUnknownExceptionMessage(Exception ex)
        {
            throw ex;
        }
    }
}
