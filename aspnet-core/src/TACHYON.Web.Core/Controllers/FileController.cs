using Abp.AspNetCore.Mvc.Authorization;
using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Dto;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Storage;
using TACHYON.Waybills;

namespace TACHYON.Web.Controllers
{
    public class FileController : TACHYONControllerBase
    {
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IRepository<RoutPoint, long> _routPointRepository;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly WaybillsManager _waybillsManager;

        public FileController(
            ITempFileCacheManager tempFileCacheManager,
            IBinaryObjectManager binaryObjectManager
, IRepository<RoutPoint, long> routPointRepository, WaybillsManager waybillsManager, IRepository<DocumentFile, Guid> documentFileRepository)
        {
            _tempFileCacheManager = tempFileCacheManager;
            _binaryObjectManager = binaryObjectManager;
            _routPointRepository = routPointRepository;
            _waybillsManager = waybillsManager;
            _documentFileRepository = documentFileRepository;
        }

        [DisableAuditing]
        public ActionResult DownloadTempFile(FileDto file)
        {
            var fileBytes = _tempFileCacheManager.GetFile(file.FileToken);

            if (fileBytes == null)
            {
                return NotFound(L("RequestedFileDoesNotExists"));
            }
             MimeTypes.TryGetExtension(file.FileType,out var exten);

            file.FileName = file.FileName +"."+ exten;
            return File(fileBytes, file.FileType, file.FileName);
        }

        [DisableAuditing]
        public async Task<ActionResult> DownloadBinaryFile(Guid id, string contentType, string fileName)
        {
            var fileObject = await _binaryObjectManager.GetOrNullAsync(id);
            if (fileObject == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }

            return File(fileObject.Bytes, contentType, fileName);
        }

        [DisableAuditing]
        [AbpMvcAuthorize()]

        public async Task<ActionResult> DownloadPDOFile(long id)
        {
            DisableTenancyFilters();
           // var Point = await _routPointRepository.GetAll().Include(e=>e.RoutPointDocuments).FirstOrDefaultAsync(x => x.Id == id && x.IsComplete && x.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId);
            var POD =await _routPointDocumentRepository.FirstOrDefaultAsync(x => x.RoutePointDocumentType == RoutePointDocumentType.POD && x.RoutPointId == id && x.RoutPointFk.IsComplete && x.RoutPointFk.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId);
            if (POD == null)
            {
                throw new UserFriendlyException(L("ThePODDocumentIsNotFound"));

            }
            var binaryObject = await _binaryObjectManager.GetOrNullAsync(POD.DocumentId.Value);
            var file = new FileDto(POD.DocumentName, POD.DocumentContentType);

            MimeTypes.TryGetExtension(file.FileType, out var exten);

            file.FileName = file.FileName + "." + exten;
            return File(binaryObject.Bytes, file.FileType, file.FileName);
        }

        [DisableAuditing]
        [AbpMvcAuthorize()]
        public async Task<ActionResult> DownloadTripAttachmentFile(int id)
        {
            DisableTenancyFilters();
            var documentFile = await _documentFileRepository.FirstOrDefaultAsync(x => x.ShippingRequestTripId==id && x.ShippingRequestTripFk.HasAttachment);
            if(documentFile == null)
            {
                throw new UserFriendlyException(L("TheFileIsNotFound"));
            }

            var binaryObject = await _binaryObjectManager.GetOrNullAsync(documentFile.BinaryObjectId.Value);
            var file = new FileDto(documentFile.Name, documentFile.Extn);

            MimeTypes.TryGetExtension(file.FileType, out var exten);

            file.FileName = file.FileName + "." + exten;
            return File(binaryObject.Bytes, file.FileType, file.FileName);
        }
            [AbpMvcAuthorize()]
        public ActionResult waybill(int id)
        {

           var bytes = _waybillsManager.GetSingleDropOrMasterWaybillPdf(id);

            MimeTypes.TryGetExtension("application/pdf", out var exten);

            return File(bytes, "application/pdf", "waybill.pdf");
        }

        [AbpMvcAuthorize()]
        public ActionResult DropWaybill(int id)
        {
            var bytes = _waybillsManager.GetMultipleDropWaybillPdf(id);

            MimeTypes.TryGetExtension("application/pdf", out var exten);

            return File(bytes, "application/pdf", "DropWaybill.pdf");
        }
    }
}