using Abp.AspNetCore.Mvc.Authorization;
using Abp.AspNetZeroCore.Net;
using Abp.BackgroundJobs;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Common;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Dto;
using TACHYON.Localization.Importing;
using TACHYON.Shipping.Drivers;
using TACHYON.Shipping.Drivers.Dto;
using TACHYON.Shipping.Trips;
using TACHYON.Shipping.Trips.Accidents;
using TACHYON.Shipping.Trips.Accidents.Dto;
using TACHYON.Storage;
using TACHYON.Trucks;
using TACHYON.Trucks.Dtos;
using TACHYON.Trucks.Importing.Dto;

namespace TACHYON.Web.Controllers
{

    public class HelperController : TACHYONControllerBase
    {
        private readonly TrucksAppService _trucksAppService;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBinaryObjectManager BinaryObjectManager;
        protected readonly IBackgroundJobManager BackgroundJobManager;

        private const int MaxDocumentFilePictureSize = 5242880; //5MB

        private ShippingRequestDriverManager _shippingRequestDriverManager;
        private ShippingRequestsTripManager _shippingRequestTripManger;
        private CommonManager _commonManager;
        private IShippingRequestTripAccidentAppService _shippingRequestTripAccidentAppService;
        public HelperController(TrucksAppService trucksAppService, ITempFileCacheManager tempFileCacheManager, IBinaryObjectManager binaryObjectManager, IBackgroundJobManager backgroundJobManager,
            ShippingRequestDriverManager shippingRequestDriverManager,
            CommonManager commonManager, ShippingRequestsTripManager shippingRequestTripManger, IShippingRequestTripAccidentAppService shippingRequestTripAccidentAppService)
        {
            _trucksAppService = trucksAppService;
            _tempFileCacheManager = tempFileCacheManager;
            BinaryObjectManager = binaryObjectManager;
            BackgroundJobManager = backgroundJobManager;
            _shippingRequestDriverManager = shippingRequestDriverManager;
            _commonManager = commonManager;
            _shippingRequestTripManger = shippingRequestTripManger;
            _shippingRequestTripAccidentAppService = shippingRequestTripAccidentAppService;
        }

        public async Task<FileResult> GetTruckPictureByTruckId(long truckId)
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


        [HttpPost]
        [AbpMvcAuthorize(AppPermissions.Pages_AppLocalization_Edit)]
        public async Task<JsonResult> ImportTerminologyFromExcel()
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

                await BackgroundJobManager.EnqueueAsync<ImportTerminologyToExcelJob, byte[]>(fileBytes);

                return Json(new AjaxResponse(new { }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpPost]
        [AbpMvcAuthorize()]
        // [Produces("application/json")]
        [Route("/api/services/app/ShippingRequestDriver/UploadPointDeliveryDocument")]
        public async Task<JsonResult> SetDropOffPointToDelivery(long? pointId)
        {
            try
            {
                var file = Request.Form.Files.First();
                //Input.Document = file;
                if (file.Length == 0)
                {
                    throw new UserFriendlyException(L("File_Empty_Error"));
                }

                if (file.Length > 1048576 * 100) //100 MB
                {
                    throw new UserFriendlyException(L("File_SizeLimit_Error"));
                }

                var document = await _commonManager.UploadDocument(file, AbpSession.TenantId);

                await _shippingRequestDriverManager.SetPointToDelivery(document, pointId);
                return Json(new AjaxResponse(new { }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpPost]
        [AbpMvcAuthorize()]
        // [Produces("application/json")]
        [Route("/api/services/app/ShippingRequestDriver/UploadDeliveryNoteDocument")]
        public async Task<JsonResult> UploadDeliveryNoteDocument(long? pointId)
        {
            try
            {
                var file = Request.Form.Files.First();
                //Input.Document = file;
                if (file.Length == 0)
                {
                    throw new UserFriendlyException(L("File_Empty_Error"));
                }

                if (file.Length > 1048576 * 100) //100 MB
                {
                    throw new UserFriendlyException(L("File_SizeLimit_Error"));
                }

                var document = await _commonManager.UploadDocument(file, AbpSession.TenantId);
                await _shippingRequestDriverManager.UploadDeliveryNote(document, pointId);

                return Json(new AjaxResponse(new { }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }



        [HttpPost]
        [AbpMvcAuthorize()]
        [Route("/api/services/app/ShippingRequestDriver/UploadDeliveredGoodPicture")]
        public async Task<JsonResult> UploadDeliveredGoodPicture(long? pointId)
        {
            try
            {
                var file = Request.Form.Files.First();
                if (file.Length == 0)
                {
                    throw new UserFriendlyException(L("File_Empty_Error"));
                }

                if (file.Length > 1048576 * 100) //100 MB
                {
                    throw new UserFriendlyException(L("File_SizeLimit_Error"));
                }

                var document = await _commonManager.UploadDocument(file, AbpSession.TenantId);
                await _shippingRequestDriverManager.UploadDeliveredGoodPicture(document, pointId);

                return Json(new AjaxResponse(new { }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }


        [HttpPost]
        [AbpMvcAuthorize()]
        [Route("/api/services/app/ShippingRequestDriver/AddIncidentReport")]
        public async Task<JsonResult> AddIncidentReport(CreateOrEditShippingRequestTripAccidentDto input)
        {
            try
            {
                var file = Request.Form.Files.First();
                if (file != null)
                {
                    if (file.Length == 0)
                    {
                        throw new UserFriendlyException(L("File_Empty_Error"));
                    }

                    if (file.Length > 1048576 * 100) //100 MB
                    {
                        throw new UserFriendlyException(L("File_SizeLimit_Error"));
                    }

                    var document = await _commonManager.UploadDocument(file, AbpSession.TenantId);
                    input.DocumentName = document.DocumentName;
                    input.DocumentContentType = "image/jpeg";
                    input.DocumentId = document.DocumentId;
                }
                await _shippingRequestTripAccidentAppService.CreateOrEdit(input);

                return Json(new AjaxResponse(new { }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }


        [HttpPost]
        [AbpMvcAuthorize()]
        // [Produces("application/json")]
        [Route("/api/services/app/DropOffPointToDelivery")]
        public async Task<JsonResult> DropOffPointToDelivery(long id)
        {
            try
            {
                var file = Request.Form.Files.First();
                //Input.Document = file;
                if (file.Length == 0)
                {
                    throw new UserFriendlyException(L("File_Empty_Error"));
                }

                if (file.Length > 1048576 * 100) //100 MB
                {
                    throw new UserFriendlyException(L("File_SizeLimit_Error"));
                }

                var document = await _commonManager.UploadDocument(file, AbpSession.TenantId);
                await _shippingRequestTripManger.ConfirmPointToDelivery(document, id);
                return Json(new AjaxResponse(new { }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
    }
}