using Abp.AspNetCore.Mvc.Authorization;
using Abp.AspNetZeroCore.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Authorization.Users.Profile;
using TACHYON.Storage;

namespace TACHYON.Web.Controllers
{
    [Authorize]
    public class ProfileController : ProfileControllerBase
    {
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly UserManager _userManager;
        public ProfileController(
            ITempFileCacheManager tempFileCacheManager,
            IProfileAppService profileAppService,
            IBinaryObjectManager binaryObjectManager,
            UserManager userManager) :
            base(tempFileCacheManager, profileAppService)
        {
            _binaryObjectManager = binaryObjectManager;
            _userManager = userManager;
        }



        [AbpMvcAuthorize()]
        public async Task<FileResult> GetProfilePicture()
        {
            var user = await _userManager.FindByIdAsync(AbpSession.UserId.ToString());

            var defaultProfilePicture = "/Common/Images/SampleProfilePics/sample-profile-01.jpg";
            var file = await _binaryObjectManager.GetOrNullAsync(user.ProfilePictureId.Value);
            if (file == null)
            {
                return File(defaultProfilePicture, MimeTypeNames.ImageJpeg);
            }

            return File(file.Bytes, MimeTypeNames.ImageJpeg);
        }
    }
}