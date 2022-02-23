using Abp.AspNetCore.Mvc.Authorization;
using Abp.AspNetZeroCore.Net;
using Abp.BackgroundJobs;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp.Runtime.Session;
using Abp.Runtime.Validation;
using Abp.UI;
using Abp.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Cities;
using TACHYON.Common;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Dto;
using TACHYON.Localization.Importing;
using TACHYON.Polygons;
using TACHYON.Shipping.Drivers;
using TACHYON.Shipping.Trips.Accidents;
using TACHYON.Shipping.Trips.Accidents.Dto;
using TACHYON.Storage;
using TACHYON.Tracking;
using TACHYON.Tracking.Dto;
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
        private readonly CityManager _cityManager;
        protected readonly IBackgroundJobManager BackgroundJobManager;
        private const int _100Megabyte = 1048576 * 100;

        private const int MaxDocumentFilePictureSize = 5242880; //5MB

        private ShippingRequestDriverManager _shippingRequestDriverManager;
        private ShippingRequestPointWorkFlowProvider _workFlowProvider;
        private CommonManager _commonManager;
        private IShippingRequestTripAccidentAppService _shippingRequestTripAccidentAppService;
        public HelperController(TrucksAppService trucksAppService, ITempFileCacheManager tempFileCacheManager, IBinaryObjectManager binaryObjectManager, IBackgroundJobManager backgroundJobManager,
            ShippingRequestDriverManager shippingRequestDriverManager,
            CommonManager commonManager, ShippingRequestPointWorkFlowProvider workFlowProvider, IShippingRequestTripAccidentAppService shippingRequestTripAccidentAppService, CityManager cityManager)
        {
            _trucksAppService = trucksAppService;
            _tempFileCacheManager = tempFileCacheManager;
            BinaryObjectManager = binaryObjectManager;
            BackgroundJobManager = backgroundJobManager;
            _shippingRequestDriverManager = shippingRequestDriverManager;
            _commonManager = commonManager;
            _workFlowProvider = workFlowProvider;
            _shippingRequestTripAccidentAppService = shippingRequestTripAccidentAppService;
            _cityManager = cityManager;
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

                if (file.Length > _100Megabyte) //100 MB
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

        [AbpMvcAuthorize(AppPermissions.Pages_Administration_PolygonsImport)]
        [HttpPost]
        [Route("/api/services/app/helper/ImportCitiesPolygon")]
        public async Task<IActionResult> ImportCitiesPolygon([FromForm] IFormFile file)
        {
            if (file == null || file.Length < 1)
                throw new AbpValidationException(L("File_Empty_Error"));
            if (file.Length > _100Megabyte )
                throw new AbpValidationException(L("File_SizeLimit_Error"));

            using var stream = new StreamReader(file.OpenReadStream());
            var fileContent = await stream.ReadToEndAsync();

            try
            {
                var root = JsonConvert.DeserializeObject<PolygonListRoot>(fileContent);
                if (root == null) throw new Exception();
                
                    var polygonCitiesList = root.Features;
                    if (polygonCitiesList is null || polygonCitiesList.Count < 1) throw new Exception();
                    
                    var errorWhenImportItems =  await _cityManager.ImportAllPolygonsIntoCities(polygonCitiesList);
                    
                    return Ok(L("CitiesPolygonImportedSuccessfully")+"\n Failed To Import Items: \n"+errorWhenImportItems);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message,e);
                throw new UserFriendlyException(L("ErrorWhenReadFile"));
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
               // await _shippingRequestDriverManager.UploadDeliveredGoodPicture(document, pointId);

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
        public async Task<JsonResult> DropOffPointToDelivery(InvokeStatusInputDto input)
        {
            try
            {
                var files = Request.Form.Files;
                if (files.Any(f => f.Length == 0))
                    throw new UserFriendlyException(L("File_Empty_Error"));

                if (files.Any(f => f.Length > 1048576 * 100)) //100 MB
                    throw new UserFriendlyException(L("File_SizeLimit_Error"));

                var documents = await _commonManager.UploadDocuments(files, AbpSession.TenantId);
                var args = new PointTransactionArgs { PointId = input.Id, Documents = documents };
                await _workFlowProvider.Invoke(args, input.Action);
                return Json(new AjaxResponse(new { }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
    }
}