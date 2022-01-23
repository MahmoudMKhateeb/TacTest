using Abp.AspNetCore.Mvc.Authorization;
using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Documents.DocumentTypes;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Storage;
using TACHYON.Tracking;
using TACHYON.Waybills;

namespace TACHYON.Web.Controllers
{
    public class FileController : TACHYONControllerBase
    {
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IRepository<RoutPoint, long> _routPointRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly UserManager _userManager;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly WaybillsManager _waybillsManager;
        private readonly IRepository<RoutPointDocument, long> _routPointDocumentRepository;
        private readonly IRepository<ShippingRequestTripAccident> _shippingRequestTripAccidentRepository;
        private readonly ShippingRequestPointWorkFlowProvider _workflow;
        public FileController(
            ITempFileCacheManager tempFileCacheManager,
            IBinaryObjectManager binaryObjectManager
, IRepository<RoutPoint, long> routPointRepository, WaybillsManager waybillsManager, IRepository<DocumentFile, Guid> documentFileRepository, IRepository<RoutPointDocument, long> routPointDocumentRepository, IRepository<ShippingRequestTrip> shippingRequestTripRepository, UserManager userManager, IRepository<ShippingRequestTripAccident> shippingRequestTripAccidentRepository, ShippingRequestPointWorkFlowProvider workflow)
        {
            _tempFileCacheManager = tempFileCacheManager;
            _binaryObjectManager = binaryObjectManager;
            _routPointRepository = routPointRepository;
            _waybillsManager = waybillsManager;
            _documentFileRepository = documentFileRepository;
            _routPointDocumentRepository = routPointDocumentRepository;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _userManager = userManager;
            _shippingRequestTripAccidentRepository = shippingRequestTripAccidentRepository;
            _workflow = workflow;
        }
        [DisableAuditing]
        public ActionResult DownloadTempFile(FileDto file)
        {
            var fileBytes = _tempFileCacheManager.GetFile(file.FileToken);

            if (fileBytes == null)
            {
                return NotFound(L("RequestedFileDoesNotExists"));
            }
            MimeTypes.TryGetExtension(file.FileType, out var exten);

            file.FileName = file.FileName + "." + exten;
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
        public async Task<ActionResult> DownloadTripAttachmentFile(int id)
        {
            DisableTenancyFilters();
            var documentFile = new DocumentFile();
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var user = await _userManager.FindByIdAsync(AbpSession.UserId.ToString());
                documentFile = await _documentFileRepository.GetAll()
                   .WhereIf(IsEnabled(AppFeatures.Shipper), x => x.ShippingRequestTripFk.ShippingRequestFk.TenantId == AbpSession.TenantId)
                   .WhereIf(IsEnabled(AppFeatures.Carrier), x => x.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                   .WhereIf(user.IsDriver, x => x.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId)
                   .FirstOrDefaultAsync(x => x.ShippingRequestTripId == id && x.ShippingRequestTripFk.HasAttachment);
            }
            if (documentFile == null)
            {
                throw new UserFriendlyException(L("TheFileIsNotFound"));
            }

            var binaryObject = await _binaryObjectManager.GetOrNullAsync(documentFile.BinaryObjectId.Value);
            var file = new FileDto(documentFile.Name, documentFile.Extn);

            MimeTypes.TryGetExtension(file.FileType, out var exten);

            file.FileName = file.FileName + "." + exten;
            return File(binaryObject.Bytes, file.FileType, file.FileName);
        }


        [DisableAuditing]
        [AbpMvcAuthorize()]
        public async Task<ActionResult> DownloadTripIncidentFile(int id)
        {
            var Accident = await _shippingRequestTripAccidentRepository
   .GetAll()
           .Where(x => x.Id == id)
           .Where(x => x.RoutPointFK.ShippingRequestTripFk.AssignedDriverUserId == AbpSession.UserId)
   .FirstOrDefaultAsync();
            if (Accident == null) throw new UserFriendlyException(L("NoRecoredFound"));

            var binaryObject = await _binaryObjectManager.GetOrNullAsync(Accident.DocumentId.Value);
            var file = new FileDto(Accident.DocumentName, Accident.DocumentContentType);

            MimeTypes.TryGetExtension(file.FileType, out var exten);

            file.FileName = file.FileName + "." + exten;
            return File(binaryObject.Bytes, file.FileType, file.FileName);

        }
        [DisableAuditing]
        [AbpMvcAuthorize()]
        public async Task<ActionResult> DownloadPODFile(Guid documentId, string contentType, string fileName)
        {
            var fileBytes = await _binaryObjectManager.GetOrNullAsync(documentId);

            if (fileBytes == null)
                return NotFound(L("RequestedFileDoesNotExists"));

            MimeTypes.TryGetExtension(contentType, out var exten);

            fileName = fileName + "." + exten;
            return File(fileBytes.Bytes, contentType, fileName);
        }

        [AbpMvcAuthorize()]
        public ActionResult waybill(int id)
        {

            var bytes = _waybillsManager.GetSingleDropOrMasterWaybillPdf(id);

            MimeTypes.TryGetExtension(DocumentTypeConsts.PDF, out var exten);

            return File(bytes, DocumentTypeConsts.PDF, "waybill.pdf");
        }

        [AbpMvcAuthorize()]
        public ActionResult DropWaybill(int id)
        {
            var bytes = _waybillsManager.GetMultipleDropWaybillPdf(id);

            MimeTypes.TryGetExtension(DocumentTypeConsts.PDF, out var exten);

            return File(bytes, DocumentTypeConsts.PDF, "DropWaybill.pdf");
        }
    }
}