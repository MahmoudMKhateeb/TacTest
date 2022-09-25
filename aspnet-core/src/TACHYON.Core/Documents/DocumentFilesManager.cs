using Abp;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.Runtime.Validation;
using Abp.Specifications;
using Abp.Timing;
using Abp.UI;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Documents.DocumentsEntities;
using TACHYON.Documents.DocumentTypes;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;
using TACHYON.Storage;
using TACHYON.Trucks.Dtos;

namespace TACHYON.Documents
{
    public class DocumentFilesManager : TACHYONDomainServiceBase
    {
        private const int MaxDocumentFileBytes = 5242880; //5MB
        private readonly TenantManager TenantManager;
        private readonly IAppNotifier _appNotifier;


        public DocumentFilesManager(IRepository<DocumentFile, Guid> documentFileRepository,
            TenantManager tenantManager,
            IRepository<DocumentType, long> documentTypeRepository,
            ITempFileCacheManager tempFileCacheManager,
            IBinaryObjectManager binaryObjectManager,
            IUserEmailer userEmailer,
            IAppNotifier appNotifier)
        {
            _documentFileRepository = documentFileRepository;
            TenantManager = tenantManager;
            _tenantManager = tenantManager;
            _documentTypeRepository = documentTypeRepository;
            _tempFileCacheManager = tempFileCacheManager;
            _binaryObjectManager = binaryObjectManager;
            _userEmailer = userEmailer;
            AbpSession = NullAbpSession.Instance;
            _appNotifier = appNotifier;
        }

        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly IRepository<DocumentType, long> _documentTypeRepository;
        private readonly TenantManager _tenantManager;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IUserEmailer _userEmailer;
        public IAbpSession AbpSession { get; set; }


        /// <summary>
        /// list of missing required documents types from tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public async Task<List<DocumentType>> GetAllTenantMissingRequiredDocumentTypesListAsync(int tenantId)
        {
            // list of Accepted and not Rejected and not expired documents
            var activeDocuments = await GetAllTenantActiveRequiredDocumentFilesListAsync(tenantId);

            var requiredDocumentTypes = await GetAllTenantRequiredDocumentTypesListAsync(tenantId);


            if (requiredDocumentTypes == null || !requiredDocumentTypes.Any())
            {
                return requiredDocumentTypes;
            }

            foreach (var item in requiredDocumentTypes.ToList())
            {
                if (activeDocuments.Any(x => x.DocumentTypeId == item.Id))
                {
                    requiredDocumentTypes.Remove(item);
                }
            }

            return requiredDocumentTypes;
        }


        /// <summary>
        ///   list of all required documents types from tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public async Task<List<DocumentType>> GetAllTenantRequiredDocumentTypesListAsync(int tenantId)
        {
            var tenant = _tenantManager.GetById(tenantId);

            var editionId = tenant.EditionId;

            return await _documentTypeRepository.GetAll()
                .Include(x => x.Translations)
                .Where(x => (x.EditionId == editionId && !x.DocumentRelatedWithId.HasValue) ||
                            x.DocumentRelatedWithId == tenantId)
                .ToListAsync();
        }


        /// <summary>
        /// list all tenant active document Files
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>
        /// list of Accepted and not Rejected and not expired documentsFiles
        /// </returns>
        public async Task<List<DocumentFile>> GetAllTenantActiveRequiredDocumentFilesListAsync(int tenantId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                return await _documentFileRepository.GetAll()
                    .Include(doc => doc.DocumentTypeFk)
                    .Where(x => x.DocumentTypeFk.DocumentsEntityId == DocumentsEntitiesEnum.Tenant)
                    .Where(x => x.TenantId == tenantId)
                    .Where(new ActiveRequiredDocumentSpecification())
                    .ToListAsync();
            }
        }

        /// <summary>
        /// list all tenant active document Files
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>
        /// list of driver and trucks documents for tenant that it is expired and will expires within 2 months
        /// </returns>
        public async Task<List<DocumentFile>> GetAllTenantDriverAndTruckDocumentFilesListAsync(int tenantId)
        {
            DisableTenancyFilters();
            return await _documentFileRepository.GetAll()
                .Include(doc => doc.DocumentTypeFk)
                .Include(doc => doc.TruckFk)
                .Include(doc => doc.UserFk)
                .Where(x => x.DocumentTypeFk.DocumentsEntityId == DocumentsEntitiesEnum.Driver ||
                            x.DocumentTypeFk.DocumentsEntityId == DocumentsEntitiesEnum.Truck)
                .Where(x => x.TenantId == tenantId)
                .Where(x => x.DocumentTypeFk.HasExpirationDate &&
                            //get documents that is already expired and will be expired within 2 coming months
                            x.ExpirationDate.Value.Date <= DateTime.Now.Date.AddMonths(2)
                    //+DateTime.Now.Date.AddMonths(2).Date.Month <= x.ExpirationDate.Value.Date.Month
                )
                .ToListAsync();
        }


        /// <summary>
        /// list all tenant active document Files
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>
        /// list of Accepted and not Rejected and not expired documentsFiles
        /// </returns>
        public async Task SendDocumentsExpiredStatusMonthlyReport()
        {
            DisableTenancyFilters();
            var tenants = await TenantManager.Tenants.ToListAsync();
            foreach (var tenant in tenants)
            {
                var documents = await GetAllTenantDriverAndTruckDocumentFilesListAsync(tenant.Id);
                
                if (documents.Count > 0)
                    await _userEmailer.SendExpiredDocumentsEmail(tenant.Id,documents.ToArray());
            }
        }

        /// <summary>
        /// Driver Iqama DocumentFile
        /// </summary>
        /// <param name="driverId"></param>
        public async Task<DocumentFile> GetDriverIqamaActiveDocumentAsync(long driverId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                return await _documentFileRepository.GetAll()
                    .Where(x => x.UserId == driverId)
                    .Where(x => x.DocumentTypeFk.SpecialConstant ==
                                TACHYONConsts.DriverIqamaDocumentTypeSpecialConstant)
                    .FirstOrDefaultAsync();
            }
        }

        /// <summary>
        /// Truck Istimara DocumentFile
        /// </summary>
        /// <param name="driverId"></param>
        public async Task<DocumentFile> GetTruckIstimaraActiveDocumentAsync(long truckId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                return await _documentFileRepository.GetAll()
                    .Where(x => x.TruckId == truckId)
                    .Where(x => x.DocumentTypeFk.SpecialConstant ==
                                TACHYONConsts.TruckIstimaraDocumentTypeSpecialConstant.ToLower())
                    .FirstOrDefaultAsync();
            }
        }

        /// <summary>
        /// convert file from temp-cache to binary-file and save it to database
        /// </summary>
        /// <param name="fileToken"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public async Task<Guid> SaveDocumentFileBinaryObject(string fileToken, int? tenantId)
        {
            var fileBytes = _tempFileCacheManager.GetFile(fileToken);

            if (fileBytes == null)
            {
                throw new UserFriendlyException("There is no such document file with the token: " + fileToken);
            }

            if (fileBytes.Length > MaxDocumentFileBytes)
            {
                throw new UserFriendlyException(L("DocumentFile_Warn_SizeLimit",
                    TACHYONConsts.MaxDocumentFileBytesUserFriendlyValue));
            }

            var storedFile = new BinaryObject(tenantId, fileBytes);
            await _binaryObjectManager.SaveAsync(storedFile);

            return storedFile.Id;
        }

        /// <summary>
        /// For Driver and truck
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="documentsEntityId"></param>
        /// <returns></returns>
        public async Task<bool> CheckIfMissingDocumentFiles(string entityId, DocumentsEntitiesEnum documentsEntityId)
        {
            var result = false;
            switch (documentsEntityId)
            {
                case DocumentsEntitiesEnum.Driver:
                    {
                        var documentTypes = await _documentTypeRepository.GetAll()
                            .Where(doc => doc.DocumentsEntityId == DocumentsEntitiesEnum.Driver)
                            .Where(x => x.IsRequired)
                            .CountAsync();

                        var submittedDocuments = await _documentFileRepository.GetAll()
                            .Where(t => t.UserId == long.Parse(entityId))
                            .Where(x => x.DocumentTypeFk.IsRequired)
                            .CountAsync();
                        result = documentTypes != submittedDocuments;
                        break;
                    }
                case DocumentsEntitiesEnum.Truck:
                    {
                        var documentTypes = await _documentTypeRepository.GetAll()
                            .Where(doc => doc.DocumentsEntityId == DocumentsEntitiesEnum.Truck)
                            .Where(x => x.IsRequired)
                            .CountAsync();

                        var submittedDocuments = await _documentFileRepository.GetAll()
                            .Where(t => t.TruckId == long.Parse(entityId))
                            .Where(x => x.DocumentTypeFk.IsRequired)
                            .CountAsync();
                        result = documentTypes != submittedDocuments;
                        break;
                    }
            }


            return result;
        }


        public async Task CreateOrEditDocumentFile(CreateOrEditDocumentFileDto input)
        {
            if (input.Id.HasValue)
            {
                await UpdateDocumentFile(input);
                return;
            }

            await CreateDocumentFile(input);
        }

        public async Task CreateDocumentFile(CreateOrEditDocumentFileDto input)
        {
            
            var documentFile = ObjectMapper.Map<DocumentFile>(input);
            
            if (string.IsNullOrEmpty(documentFile.Name))
                documentFile.Name = input.DocumentTypeDto.DisplayName + "_" + AbpSession.GetTenantId();

            if (AbpSession.TenantId != null)
                documentFile.TenantId = AbpSession.TenantId;

            if (input.EntityType == DocumentsEntitiesEnum.Truck)
                documentFile.TruckId = long.Parse(input.EntityId);

            if (input.EntityType == DocumentsEntitiesEnum.Driver)
                documentFile.UserId = long.Parse(input.EntityId);
            
            if (input.UpdateDocumentFileInput != null)
            {
                if (!input.UpdateDocumentFileInput.FileToken.IsNullOrEmpty())
                {
                    documentFile.BinaryObjectId = await SaveDocumentFileBinaryObject(input.UpdateDocumentFileInput.FileToken, AbpSession.TenantId);
                }
            }
            await _documentFileRepository.InsertAsync(documentFile);
        }

        public async Task UpdateDocumentFile(CreateOrEditDocumentFileDto input)
        {
            DocumentFile documentFile = await _documentFileRepository
                .GetAll()
                .Include(x => x.DocumentTypeFk)
                .FirstOrDefaultAsync(x => x.Id == (Guid)input.Id);

            if (input.UpdateDocumentFileInput != null && !input.UpdateDocumentFileInput.FileToken.IsNullOrEmpty())
            {
                if (documentFile.BinaryObjectId != null)
                {
                    await _binaryObjectManager.DeleteAsync(documentFile.BinaryObjectId.Value);
                }

                input.BinaryObjectId =
                    await SaveDocumentFileBinaryObject(input.UpdateDocumentFileInput.FileToken, AbpSession.TenantId);
                input.IsAccepted = false;
                input.IsRejected = false;
            }

            if (documentFile.ExpirationDate != input.ExpirationDate)
            {
                input.IsAccepted = false;
                input.IsRejected = false;
            }

            ObjectMapper.Map(input, documentFile);
            documentFile.RejectionReason = "";
        }

        public async Task DeleteDocumentFile(DocumentFile documentFile)
        {
            if (documentFile.BinaryObjectId != null)
            {
                await _binaryObjectManager.DeleteAsync(documentFile.BinaryObjectId.Value);
            }

            await _documentFileRepository.DeleteAsync(documentFile.Id);
        }


        public async Task NotifyExpiredDocumentFile()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var docs = _documentFileRepository.GetAll()
                    .Include(x => x.DocumentTypeFk)
                    .Include(x => x.TenantFk)
                    .Where(x => x.DocumentTypeFk.HasExpirationDate)
                    .Where(x => x.IsAccepted)
                    .ToList();


                foreach (DocumentFile documentFile in docs)
                {
                    if (documentFile.ExpirationDate == null)
                    {
                        continue;
                    }

                    //AlertDays 
                    var expirationAlertDays = documentFile.DocumentTypeFk.ExpirationAlertDays;
                    if (expirationAlertDays != null)
                    {
                        var alertDate = documentFile.ExpirationDate.Value.AddDays(-1 * expirationAlertDays.Value).Date;
                        if (alertDate == Clock.Now.Date)
                        {
                            var user = new UserIdentifier(documentFile.TenantId, documentFile.CreatorUserId.Value);
                            await _appNotifier.DocumentFileBeforExpiration(user, documentFile.Id,
                                expirationAlertDays.Value);
                        }
                    }

                    //Expiration
                    if (documentFile.ExpirationDate.Value.Date == Clock.Now.Date)
                    {
                        var user = new UserIdentifier(documentFile.TenantId, documentFile.CreatorUserId.Value);
                        await _appNotifier.DocumentFileExpiration(user, documentFile.Id);
                        Logger.Info(documentFile + "ExpiredDocumentFileWorker logger.");

                        //Send email with expired documents
                        if (documentFile.TenantId.HasValue)
                         await _userEmailer.SendExpiredDocumentsEmail(documentFile.TenantId.Value, documentFile);
                    }
                }


                CurrentUnitOfWork.SaveChanges();
            }
        }
        //---R

        /// <summary>
        /// Document Accepted and not Rejected and not expired documentsFiles.
        /// </summary>
        public class ActiveRequiredDocumentSpecification : Specification<DocumentFile>
        {
            public override Expression<Func<DocumentFile, bool>> ToExpression()
            {
                return x => (x.ExpirationDate > DateTime.Now || x.ExpirationDate == null ||
                             !x.DocumentTypeFk.HasExpirationDate)
                            && (x.DocumentTypeFk.IsRequired)
                            && (!x.IsRejected)
                            && (x.IsAccepted);
            }
        }
    }
}