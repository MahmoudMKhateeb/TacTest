using Abp.Application.Features;
using Abp.Domain.Uow;
using Abp.IO.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Storage;

namespace TACHYON.Common
{
    public class CommonManager : TACHYONDomainServiceBase
    {
        private IAbpSession _AbpSession { get; set; }
        private readonly IFeatureChecker _featureChecker;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ITempFileCacheManager _tempFileCacheManager;

        public CommonManager(
            IAbpSession AbpSession,
            IFeatureChecker featureChecker,
            IBinaryObjectManager binaryObjectManager,
            ITempFileCacheManager tempFileCacheManager)
        {
            _AbpSession = AbpSession;
            _featureChecker = featureChecker;
            _binaryObjectManager = binaryObjectManager;
            _tempFileCacheManager = tempFileCacheManager;
        }
        /// <summary>
        /// This function helep developers to execute any query need to disable filter if the user is host or check if tenant have feature to execute this query
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="funcToRun">the method it will</param>
        /// <param name="featurename">The permission filter give to tenant</param>
        /// <returns></returns>
        public T ExecuteMethodIfHostOrTenantUsers<T>(Func<T> funcToRun, string featurename = "")
        {

            if (_AbpSession.TenantId.HasValue && (string.IsNullOrEmpty(featurename) || _featureChecker.IsEnabled(featurename)))
            {
                return funcToRun();
            }
            else if (!_AbpSession.TenantId.HasValue)
            {
                using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
                {
                    return funcToRun();
                }
            }
            else
            {
                throw new UserFriendlyException(L("YouDontHaveThisPermission"));
            }
            
        }

/// <summary>
/// by angular sent file upload as base64
/// </summary>
/// <param name="document">document object</param>
/// <param name="TenantId">Tenant id for user upload file</param>
/// <returns></returns>
        public async Task<IHasDocument> UploadDocumentAsBase64(IDocumentUpload document,int? TenantId)
        {    
            if (document.DocumentBase64 != null && !string.IsNullOrEmpty(document.DocumentBase64.ToString().Trim()))
            {
                var fileBytes = Convert.FromBase64String(document.DocumentBase64.Split(',')[1]);

                var fileObject = new BinaryObject(TenantId, fileBytes);
                await _binaryObjectManager.SaveAsync(fileObject);
                if (document.DocumentId.HasValue)
                {
                    await _binaryObjectManager.DeleteAsync((Guid)document.DocumentId);

                }
                document.DocumentId = fileObject.Id;

            }
            return document;
        }

        public async Task<IHasDocument> UploadDocument(IFormFile File, int? TenantId)
        {
            IHasDocument document = new HasDocument();
            if (File.Length>0)
            {
                byte[] fileBytes;
                using (var stream = File.OpenReadStream())
                {
                    fileBytes = stream.GetAllBytes();
                }
                var fileObject = new BinaryObject(TenantId, fileBytes);
                await _binaryObjectManager.SaveAsync(fileObject);
                document.DocumentId = fileObject.Id;
                document.DocumentContentType = File.ContentType;
                document.DocumentName = Path.GetFileName(File.FileName).Split(".")[0];
            }
            return document;
        }
        
        /// <summary>
        /// Delete file attachment from database
        /// </summary>
        /// <param name="DocumentId">BinaryObject id</param>
        /// <returns></returns>
        public async Task DeleteDocument(Guid DocumentId)
        {
            await _binaryObjectManager.DeleteAsync(DocumentId);

        }
        /// <summary>
        /// Get document file to download
        /// </summary>
        /// <param name="Document"></param>
        /// <returns></returns>

        public async Task<FileDto> GetDocument(IHasDocument Document)

        {
            var binaryObject = await _binaryObjectManager.GetOrNullAsync(Document.DocumentId.Value);

            var file = new FileDto(Document.DocumentName, Document.DocumentContentType);

            _tempFileCacheManager.SetFile(file.FileToken, binaryObject.Bytes);

            return file;
        }
    }
}
