using Abp.Dependency;
using DevExpress.AspNetCore.Reporting.QueryBuilder;
using DevExpress.AspNetCore.Reporting.QueryBuilder.Native.Services;
using Microsoft.AspNetCore.Mvc;


namespace TACHYON.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]  
    [Route("DXXQB")] 
    public class CustomQueryBuilderController : QueryBuilderController, ITransientDependency
    {
        public CustomQueryBuilderController(IQueryBuilderMvcControllerService controllerService) : base(controllerService)
        {
            
        }
    }
}