using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using System;

namespace TACHYON.Web.Services.ExceptionHandlers
{
    public class CustomWebDocumentViewerExceptionHandler : WebDocumentViewerExceptionHandler
    {
        public override string GetFaultExceptionMessage(FaultException ex)
        {
            throw ex;
        }

        public override string GetDocumentCreationExceptionMessage(DocumentCreationException ex)
        {
            throw ex;
        }

        public override string GetUnknownExceptionMessage(Exception ex)
        {
            throw ex;
        }
    }
}
