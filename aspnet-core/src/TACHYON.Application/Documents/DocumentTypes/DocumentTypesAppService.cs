using Abp.Application.Editions;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Documents.DocumentsEntities;
using TACHYON.Documents.DocumentTypes.Dtos;
using TACHYON.Documents.DocumentTypes.Exporting;
using TACHYON.Documents.DocumentTypeTranslations;
using TACHYON.Documents.DocumentTypeTranslations.Dtos;
using TACHYON.Dto;
using TACHYON.MultiTenancy;
using TACHYON.Storage;

namespace TACHYON.Documents.DocumentTypes
{
    [AbpAuthorize(AppPermissions.Pages_DocumentTypes)]
    public class DocumentTypesAppService : TACHYONAppServiceBase
    {
        private readonly IRepository<DocumentType, long> _documentTypeRepository;
        private readonly IDocumentTypesExcelExporter _documentTypesExcelExporter;
        //private readonly IRepository<DocumentsEntity, int> _documentsEntityRepository;
        private readonly IRepository<DocumentTypeTranslation> _documentTypeTranslationRepository;
        //private readonly IRepository<DocumentsEntity, int> _documentEntityRepository;
        private readonly IRepository<Tenant> _Tenant;
        private readonly IRepository<Edition, int> _editionRepository;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly DocumentFilesManager _documentFilesManager;


        public DocumentTypesAppService(
            //IRepository<DocumentsEntity, int> documentEntityRepository,
            IRepository<DocumentType, long> documentTypeRepository,
            IDocumentTypesExcelExporter documentTypesExcelExporter,
            //IRepository<DocumentsEntity, int> documentsEntityRepository,
            IRepository<DocumentTypeTranslation> documentTypeTranslationRepository,
            IRepository<Edition, int> editionRepository,
            IBinaryObjectManager BinaryObjectManager,
            ITempFileCacheManager tempFileCacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Tenant> Tenant,
            DocumentFilesManager documentFilesManager)
        {
            _documentTypeRepository = documentTypeRepository;
            _documentTypesExcelExporter = documentTypesExcelExporter;
            //_documentsEntityRepository = documentsEntityRepository;
            _documentTypeTranslationRepository = documentTypeTranslationRepository;
            _editionRepository = editionRepository;
            //_documentEntityRepository = documentEntityRepository;
            _binaryObjectManager = BinaryObjectManager;
            _tempFileCacheManager = tempFileCacheManager;
            _unitOfWorkManager = unitOfWorkManager;
            _Tenant = Tenant;
            _documentFilesManager = documentFilesManager;
        }

        public async Task<LoadResult> GetAll(GetAllDocumentTypesInput input)
        {
            IQueryable<DocumentTypeDto> filteredDocumentFiles = _documentTypeRepository
                .GetAll()
                .ProjectTo<DocumentTypeDto>(
                    AutoMapperConfigurationProvider); // goto:#Mapper_DocumentType_DocumentTypeDto

            return await LoadResultAsync(filteredDocumentFiles, input.Filter);
        }


        public async Task<GetDocumentTypeForViewDto> GetDocumentTypeForView(long id)
        {
            var documentType = await _documentTypeRepository.GetAsync(id);


            var output =
                new GetDocumentTypeForViewDto { DocumentType = ObjectMapper.Map<DocumentTypeDto>(documentType) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentTypes_Edit)]
        public async Task<GetDocumentTypeForEditOutput> GetDocumentTypeForEdit(EntityDto<long> input)
        {
            var documentType = await _documentTypeRepository.FirstOrDefaultAsync(input.Id);
            var documentTypeDto = ObjectMapper.Map<CreateOrEditDocumentTypeDto>(documentType);


            if (documentType.DocumentRelatedWithId.HasValue)
            {
                documentTypeDto.DocumentRelatedWith = new SelectItemDto(documentType.DocumentRelatedWithId.ToString(),
                    (await _Tenant.SingleAsync(t => t.Id == documentType.DocumentRelatedWithId)).Name);
            }

            var output = new GetDocumentTypeForEditOutput { DocumentType = documentTypeDto };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditDocumentTypeDto input)
        {
            ValidateDisplayNameDuplication(input.DisplayName, input.Id);

            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentTypes_Create)]
        protected virtual async Task Create(CreateOrEditDocumentTypeDto input)
        {
            var documentType = ObjectMapper.Map<DocumentType>(input);

            if (!input.FileToken.IsNullOrEmpty())
            {
                documentType.TemplateId =
                    await _documentFilesManager.SaveDocumentFileBinaryObject(input.FileToken, AbpSession.TenantId);
            }

            await _documentTypeRepository.InsertAsync(documentType);
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentTypes_Edit)]
        protected virtual async Task Update(CreateOrEditDocumentTypeDto input)
        {
            var documentType = await _documentTypeRepository.FirstOrDefaultAsync((long)input.Id);

            if (!input.FileToken.IsNullOrEmpty())
            {
                if (input.TemplateId != null)
                {
                    await _binaryObjectManager.DeleteAsync(input.TemplateId.Value);
                }

                input.TemplateId =
                    await _documentFilesManager.SaveDocumentFileBinaryObject(input.FileToken, AbpSession.TenantId);
            }

            ObjectMapper.Map(input, documentType);
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentTypes_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var documentType = await _documentTypeRepository.FirstOrDefaultAsync(input.Id);

            if (documentType.TemplateId != null)
            {
                await _binaryObjectManager.DeleteAsync(documentType.TemplateId.Value);
            }

            await _documentTypeRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentTypes_Edit)]
        public async Task DeleteTemplate(long Id)
        {
            var documentType = await _documentTypeRepository.FirstOrDefaultAsync(Id);
            if (documentType != null)
            {
                await _binaryObjectManager.DeleteAsync((Guid)documentType.TemplateId);

                documentType.TemplateId = null;
                documentType.TemplateContentType = null;
                documentType.TemplateName = null;
            }
        }

        //public async Task<FileDto> GetDocumentTypesToExcel(GetAllDocumentTypesForExcelInput input)
        //{
        //    var filteredDocumentTypes = _documentTypeRepository.GetAll()
        //        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
        //            e => false || e.DisplayName.Contains(input.Filter) ||
        //                 e.DocumentsEntityId.DisplayName.Contains(input.Filter))
        //        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),
        //            e => e.DisplayName == input.DisplayNameFilter)
        //        .WhereIf(input.IsRequiredFilter > -1,
        //            e => (input.IsRequiredFilter == 1 && e.IsRequired) ||
        //                 (input.IsRequiredFilter == 0 && !e.IsRequired))
        //        .WhereIf(input.HasExpirationDateFilter > -1,
        //            e => (input.HasExpirationDateFilter == 1 && e.HasExpirationDate) ||
        //                 (input.HasExpirationDateFilter == 0 && !e.HasExpirationDate))
        //        .WhereIf(input.RequiredFromFilter.HasValue, e => e.DocumentsEntityId == (int)input.RequiredFromFilter);

        //    var query = (from o in filteredDocumentTypes
        //        select new GetDocumentTypeForViewDto()
        //        {
        //            DocumentType = new DocumentTypeDto
        //            {
        //                DisplayName = o.DisplayName,
        //                IsRequired = o.IsRequired,
        //                HasExpirationDate = o.HasExpirationDate,
        //                RequiredFrom = o.DocumentsEntityFk.DisplayName,
        //                Id = o.Id
        //            }
        //        });


        //    var documentTypeListDtos = await query.ToListAsync();

        //    return _documentTypesExcelExporter.ExportToFile(documentTypeListDtos);
        //}

        //public async Task<List<SelectItemDto>> GetAllDocumentsEntitiesForTableDropdown()
        //{
        //    return await _documentsEntityRepository.GetAll()
        //        .Select(x => new SelectItemDto
        //        {
        //            Id = x.Id.ToString(),
        //            DisplayName = x == null || x.DisplayName == null ? "" : x.DisplayName.ToString()
        //        }).ToListAsync();
        //}

        public async Task Translate(long documentTypeId, DocumentTypeTranslationDto input)
        {
            var translation = await _documentTypeTranslationRepository.GetAll()
                .FirstOrDefaultAsync(pt => pt.CoreId == documentTypeId && pt.Language == input.Language);

            if (translation == null)
            {
                var newTranslation = ObjectMapper.Map<DocumentTypeTranslation>(input);
                newTranslation.CoreId = documentTypeId;

                await _documentTypeTranslationRepository.InsertAsync(newTranslation);
            }
            else
            {
                ObjectMapper.Map(input, translation);
            }
        }


        public IEnumerable<ISelectItemDto> GetAutoCompleteTenants(int editionId, string name = null)
        {
            name = name?.ToLower().Trim();
            var result =
                _Tenant.GetAll()
                    .Where(x => x.IsActive)
                    .WhereIf(!name.IsNullOrEmpty(),
                        t => t.Name.ToLower().Contains(name) || t.TenancyName.ToLower().Contains(name))
                    .Where(t => t.Edition.Id == editionId)
                    .Select(t => new SelectItemDto { DisplayName = t.Name, Id = t.Id.ToString() }).ToList();

            return result;
        }

        //public async Task<List<SelectItemDto>> GetDocumentEntitiesForTableDropdown()
        //{
        //    return await _documentEntityRepository
        //        .GetAll()
        //        .Select(x => new SelectItemDto { DisplayName = x.DisplayName, Id = x.Id.ToString() }
        //        ).ToListAsync();
        //}

        public async Task<List<SelectItemDto>> GetEditionsForTableDropdown()
        {
            return await _editionRepository.GetAll()
                .Select(x => new SelectItemDto { Id = x.Id.ToString(), DisplayName = x.DisplayName }).ToListAsync();
        }


        private bool ValidateDisplayNameDuplication(string documentTypeName, long? id)
        {
            var result = _documentTypeRepository
                .FirstOrDefault(x =>
                    x.DisplayName.Trim().ToLower() == documentTypeName.Trim().ToLower() && x.Id != id &&
                    x.DocumentRelatedWithId == null);

            if (result == null)
            {
                return true;
            }

            throw new UserFriendlyException(L("DuplicateDocumentTypeName"));
        }

        [AbpAllowAnonymous]
        public async Task<FileDto> GetFileDto(long Id)

        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant,
                       AbpDataFilters.MayHaveTenant))
            {
                var document = await _documentTypeRepository.SingleAsync(t => t.Id == Id);
                if (document == null)
                {
                    throw new UserFriendlyException(L("TheRequestNotFound"));
                }

                var binaryObject = await _binaryObjectManager.GetOrNullAsync(document.TemplateId.Value);

                var file = new FileDto(document.TemplateName, document.TemplateContentType);

                _tempFileCacheManager.SetFile(file.FileToken, binaryObject.Bytes);

                return file;
            }
        }
    }
}