using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Documents.DocumentTypes;
using TACHYON.MultiTenancy;
using TACHYON.Storage;

namespace TACHYON.Documents
{
    public class DocumentFilesManager : TACHYONDomainServiceBase
    {
        private const int MaxDocumentFileBytes = 5242880; //5MB


        public DocumentFilesManager(IRepository<DocumentFile, Guid> documentFileRepository, TenantManager tenantManager, IRepository<DocumentType, long> documentTypeRepository, ITempFileCacheManager tempFileCacheManager, IBinaryObjectManager binaryObjectManager)
        {
            _documentFileRepository = documentFileRepository;
            _tenantManager = tenantManager;
            _documentTypeRepository = documentTypeRepository;
            _tempFileCacheManager = tempFileCacheManager;
            _binaryObjectManager = binaryObjectManager;
        }

        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly IRepository<DocumentType, long> _documentTypeRepository;
        private readonly TenantManager _tenantManager;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBinaryObjectManager _binaryObjectManager;


        /// <summary>
        ///list of missing required documents types from tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public async Task<List<DocumentType>> GetAllTenantMissingRequiredDocumentTypesListAsync(int tenantId)
        {
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
        ///     list of all required documents types from tenant
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
        ///     list all tenant active document Files
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public async Task<List<DocumentFile>> GetAllTenantActiveRequiredDocumentFilesListAsync(int tenantId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                return await _documentFileRepository.GetAll()
                    .Where(x => x.TenantId == tenantId)
                    //.Where(x => x.ExpirationDate > DateTime.Now || x.ExpirationDate == null || !x.DocumentTypeFk.HasExpirationDate)
                    //.Where(x => x.DocumentTypeFk.IsRequired)
                    //.Where(x => !x.IsRejected)
                    //.Where(x => x.IsAccepted)
                    .ToListAsync();
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
    }
}