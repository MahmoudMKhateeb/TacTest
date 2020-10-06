using Abp.AspNetZeroCore.Net;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp.UI;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Dto;
using TACHYON.Storage;
using TACHYON.Trucks;

namespace TACHYON.Web.Controllers
{

    public class HelperController : TACHYONControllerBase
    {
        private readonly ITrucksAppService _trucksAppService;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private const int MaxDocumentFilePictureSize = 5242880; //5MB


        public HelperController(ITrucksAppService trucksAppService, ITempFileCacheManager tempFileCacheManager)
        {
            _trucksAppService = trucksAppService;
            _tempFileCacheManager = tempFileCacheManager;
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

    }
}