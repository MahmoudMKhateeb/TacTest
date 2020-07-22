using Abp.AspNetCore.Mvc.Authorization;
using Abp.BackgroundJobs;
using TACHYON.Authorization;
using TACHYON.Storage;

namespace TACHYON.Web.Controllers
{
    [AbpMvcAuthorize(AppPermissions.Pages_Administration_Users)]
    public class UsersController : UsersControllerBase
    {
        public UsersController(IBinaryObjectManager binaryObjectManager, IBackgroundJobManager backgroundJobManager)
            : base(binaryObjectManager, backgroundJobManager)
        {
        }
    }
}