using Abp.AspNetCore.Mvc.Authorization;
using Abp.AspNetZeroCore.Net;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp.UI;
using Abp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Authorization.Users.Profile;
using TACHYON.Authorization.Users.Profile.Dto;
using TACHYON.Dto;
using TACHYON.Storage;
using TACHYON.Web.Helpers;

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

        [HttpPost]
        [AbpMvcAuthorize()]
        public async Task<JsonResult> UploadMobileProfilePicture()
        {
            try
            {
                var profilePictureFile = Request.Form.Files.First();

                //Check input
                if (profilePictureFile == null)
                {
                    throw new UserFriendlyException(L("ProfilePicture_Change_Error"));
                }

                if (profilePictureFile.Length > MaxProfilePictureSize)
                {
                    throw new UserFriendlyException(L("ProfilePicture_Warn_SizeLimit",
                        AppConsts.MaxProfilPictureBytesUserFriendlyValue));
                }

                var extarr = new string[] { ".jpeg", ".jpg", ".png" };
                var ext = System.IO.Path.GetExtension(profilePictureFile.FileName).ToLower();
                if (!extarr.Contains(ext))
                {
                    throw new Exception(L("IncorrectImageFormat"));
                }

                byte[] fileBytes;
                using (var stream = profilePictureFile.OpenReadStream())
                {
                    fileBytes = stream.GetAllBytes();
                }


                var user = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);

                if (user.ProfilePictureId.HasValue)
                {
                    await _binaryObjectManager.DeleteAsync(user.ProfilePictureId.Value);
                }

                var storedFile = new BinaryObject(AbpSession.TenantId, fileBytes);
                await _binaryObjectManager.SaveAsync(storedFile);
                user.ProfilePictureId = storedFile.Id;

                return Json(new AjaxResponse(new { }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
    }
}