using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using TACHYON.Authorization.Users;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Documents.DocumentsEntities;
using TACHYON.Documents.DocumentTypes;
using TACHYON.MultiTenancy;
using TACHYON.Storage;

namespace TACHYON.Documents
{
    public class DocumentFilesManager : TACHYONDomainServiceBase
    {
        private const int MaxDocumentFileBytes = 5242880; //5MB


        public DocumentFilesManager(IRepository<DocumentFile, Guid> documentFileRepository, TenantManager tenantManager, IRepository<DocumentType, long> documentTypeRepository, ITempFileCacheManager tempFileCacheManager, IBinaryObjectManager binaryObjectManager, IUserEmailer userEmailer)
        {
            _documentFileRepository = documentFileRepository;
            _tenantManager = tenantManager;
            _documentTypeRepository = documentTypeRepository;
            _tempFileCacheManager = tempFileCacheManager;
            _binaryObjectManager = binaryObjectManager;
            _userEmailer = userEmailer;
        }

        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly IRepository<DocumentType, long> _documentTypeRepository;
        private readonly TenantManager _tenantManager;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IUserEmailer _userEmailer;



        /// <summary>
        /// list of missing required documents types from tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public async Task<List<DocumentType>> GetAllTenantMissingRequiredDocumentTypesListAsync(int tenantId)
        {
            // list of Accepted and not Rejected and not expired documents
            var existedList = await GetAllTenantActiveRequiredDocumentFilesListAsync(tenantId);

            var reqList = await GetAllTenantRequiredDocumentTypesListAsync(tenantId);


            if (reqList != null && reqList.Any())
            {
                foreach (var item in reqList.ToList())
                {
                    if (existedList.Any(x => x.DocumentTypeId == item.Id))
                    {
                        reqList.Remove(item);
                    }
                }
            }

            return reqList;
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
                .Where(x => x.EditionId == editionId)
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
                      .Where(x => x.DocumentTypeFk.DocumentsEntityId == (int)DocumentsEntitiesEnum.Tenant)
                    .Where(x => x.TenantId == tenantId)
                    .Where(x => x.ExpirationDate > DateTime.Now || x.ExpirationDate == null || !x.DocumentTypeFk.HasExpirationDate)
                    .Where(x => x.DocumentTypeFk.IsRequired)
                    .Where(x => !x.IsRejected)
                    .Where(x => x.IsAccepted)
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
                    .Include(doc=> doc.TruckFk)
                    .Include(doc => doc.UserFk)
                    .Where(x => x.DocumentTypeFk.DocumentsEntityId == (int)DocumentsEntitiesEnum.Driver ||
                    x.DocumentTypeFk.DocumentsEntityId == (int)DocumentsEntitiesEnum.Truck)
                    .Where(x=>x.TenantId==tenantId)
                .Where(x => x.DocumentTypeFk.HasExpirationDate &&
                //get documents that is already expired and will be expired within 2 coming months
                DateTime.Now.Date.AddMonths(2) >= x.ExpirationDate.Value.Date
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
            var tenants =await TenantManager.Tenants.ToListAsync();
            foreach (var tenant in tenants)
            {
                var documents = await GetAllTenantDriverAndTruckDocumentFilesListAsync(tenant.Id);
                if (documents.Count > 0)
                    await _userEmailer.SendDocumentsExpiredInfoAsyn(documents, tenant.Id); //documents.FirstOrDefault().TenantId.Value);
            }
        }

        /// <summary>
        ///     convert file from temp-cache to binary-file and save it to database
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
                throw new UserFriendlyException(L("DocumentFile_Warn_SizeLimit", TACHYONConsts.MaxDocumentFileBytesUserFriendlyValue));
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
                            .Where(doc => doc.DocumentsEntityId == (int)DocumentsEntitiesEnum.Driver)
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
                            .Where(doc => doc.DocumentsEntityId == (int)DocumentsEntitiesEnum.Truck)
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

    }
}