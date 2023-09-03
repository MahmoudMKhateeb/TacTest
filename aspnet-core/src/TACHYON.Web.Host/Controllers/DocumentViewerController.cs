

using Abp.Dependency;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer.Native.Services;
using Microsoft.AspNetCore.Mvc;

namespace TACHYON.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]  
    [Route("DXXRDV")] 
    public class DocumentViewerController : WebDocumentViewerController, ITransientDependency
    {
        public DocumentViewerController(IWebDocumentViewerMvcControllerService controllerService) : base(controllerService)
        {
        }
    }
}