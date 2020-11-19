using Abp.AspNetZeroCore.Net;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp.UI;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.BackgroundJobs;
using Abp.Runtime.Session;
using TACHYON.Authorization;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Dto;
using TACHYON.Storage;
using TACHYON.Trucks;
using TACHYON.Trucks.Dtos;
using TACHYON.Trucks.Importing.Dto;

namespace TACHYON.Web.Controllers
{

    public class HelperController : TACHYONControllerBase
    {
        private readonly ITrucksAppService _trucksAppService;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBinaryObjectManager BinaryObjectManager;
        protected readonly IBackgroundJobManager BackgroundJobManager;

        private const int MaxDocumentFilePictureSize = 5242880; //5MB


        public HelperController(ITrucksAppService trucksAppService, ITempFileCacheManager tempFileCacheManager, IBinaryObjectManager binaryObjectManager, IBackgroundJobManager backgroundJobManager)
        {
            _trucksAppService = trucksAppService;
            _tempFileCacheManager = tempFileCacheManager;
            BinaryObjectManager = binaryObjectManager;
            BackgroundJobManager = backgroundJobManager;
        }

        public async Task<FileResult> GetTruckPictureByTruckId(Guid truckId)
        {
            var output = await _trucksAppService.GetPictureContentForTruck(truckId);
            if (output.IsNullOrEmpty())
            {
                return File(Convert.FromBase64String(""), MimeTypeNames.ImageJpeg);
            }

            return File(Convert.FromBase64String(output), MimeTypeNames.ImageJpeg);
        }

        public UploadDocumentFileOutput UploadDocumentFile(FileDto input)
        {
            try
            {
                var File = Request.Form.Files.First();

                //Check input
                if (File == null)
                {
                    throw new UserFriendlyException(L("DocumentFileUpload_Error"));
                }

                if (File.Length > MaxDocumentFilePictureSize)
                {
                    throw new UserFriendlyException(L("DocumentFile_Warn_SizeLimit", TACHYONConsts.MaxDocumentFileBytesUserFriendlyValue));
                }

                byte[] fileBytes;
                using (var stream = File.OpenReadStream())
                {
                    fileBytes = stream.GetAllBytes();
                }


                _tempFileCacheManager.SetFile(input.FileToken, fileBytes);


                return new UploadDocumentFileOutput
                {
                    FileToken = input.FileToken,
                    FileName = input.FileName,
                    FileType = input.FileType,
                };

            }
            catch (UserFriendlyException ex)
            {
                return new UploadDocumentFileOutput(new ErrorInfo(ex.Message));
            }
        }

        [HttpPost]
        [AbpMvcAuthorize(AppPermissions.Pages_Trucks_Create)]
        public async Task<JsonResult> ImportTrucksFromExcel()
        {
            try
            {
                var file = Request.Form.Files.First();

                if (file == null)
                {
                    throw new UserFriendlyException(L("File_Empty_Error"));
                }

                if (file.Length > 1048576 * 100) //100 MB
                {
                    throw new UserFriendlyException(L("File_SizeLimit_Error"));
                }

                byte[] fileBytes;
                using (var stream = file.OpenReadStream())
                {
                    fileBytes = stream.GetAllBytes();
                }

                var tenantId = AbpSession.TenantId;
                var fileObject = new BinaryObject(tenantId, fileBytes);

                await BinaryObjectManager.SaveAsync(fileObject);

                await BackgroundJobManager.EnqueueAsync<ImportTrucksToExcelJob, ImportTrucksFromExcelJobArgs>(new ImportTrucksFromExcelJobArgs
                {
                    TenantId = tenantId,
                    BinaryObjectId = fileObject.Id,
                    User = AbpSession.ToUserIdentifier()
                });

                return Json(new AjaxResponse(new { }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

    }
}