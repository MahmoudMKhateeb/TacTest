

using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Editions;
using TACHYON.Authorization;
using TACHYON.Documents.DocumentsEntities;
using TACHYON.Documents.DocumentsEntities.Dtos;
using TACHYON.Documents.DocumentTypes.Dtos;
using TACHYON.Documents.DocumentTypes.Exporting;
using TACHYON.Documents.DocumentTypeTranslations;
using TACHYON.Documents.DocumentTypeTranslations.Dtos;
using TACHYON.Dto;

namespace TACHYON.Documents.DocumentTypes
{
    [AbpAuthorize(AppPermissions.Pages_DocumentTypes)]
    public class DocumentTypesAppService : TACHYONAppServiceBase, IDocumentTypesAppService
    {
        private readonly IRepository<DocumentType, long> _documentTypeRepository;
        private readonly IDocumentTypesExcelExporter _documentTypesExcelExporter;
        private readonly IRepository<DocumentsEntity, int> _documentsEntityRepository;
        private readonly IRepository<DocumentTypeTranslation> _documentTypeTranslationRepository;
        private readonly IRepository<DocumentsEntity, int> _documentEntityRepository;
        private readonly IRepository<Edition, int> _editionRepository;



        public DocumentTypesAppService(IRepository<DocumentsEntity, int> documentEntityRepository, IRepository<DocumentType, long> documentTypeRepository, IDocumentTypesExcelExporter documentTypesExcelExporter, IRepository<DocumentsEntity, int> documentsEntityRepository, IRepository<DocumentTypeTranslation> documentTypeTranslationRepository, IRepository<Edition, int> editionRepository)
        {
            _documentTypeRepository = documentTypeRepository;
            _documentTypesExcelExporter = documentTypesExcelExporter;
            _documentsEntityRepository = documentsEntityRepository;
            _documentTypeTranslationRepository = documentTypeTranslationRepository;
            _editionRepository = editionRepository;
            _documentEntityRepository = documentEntityRepository;
        }

        public async Task<PagedResultDto<GetDocumentTypeForViewDto>> GetAll(GetAllDocumentTypesInput input)
        {

            var filteredDocumentTypes = _documentTypeRepository.GetAll()
                        .Include(e => e.Translations)
                        .Include(x => x.DocumentsEntityFk)
                        .Include(x => x.EditionFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter) || e.DocumentsEntityFk.DisplayName.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter)
                        .WhereIf(input.IsRequiredFilter > -1, e => (input.IsRequiredFilter == 1 && e.IsRequired) || (input.IsRequiredFilter == 0 && !e.IsRequired))
                        .WhereIf(input.HasExpirationDateFilter > -1, e => (input.HasExpirationDateFilter == 1 && e.HasExpirationDate) || (input.HasExpirationDateFilter == 0 && !e.HasExpirationDate))
                        .WhereIf(input.RequiredFromFilter.HasValue, e => e.DocumentsEntityId == (int)input.RequiredFromFilter);

            var pagedAndFilteredDocumentTypes = filteredDocumentTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var documentTypeList = await pagedAndFilteredDocumentTypes.ToListAsync();


            var documentTypes = from o in documentTypeList
                                select new GetDocumentTypeForViewDto()
                                {
                                    DocumentType = ObjectMapper.Map<DocumentTypeDto>(o)
                                };


            var totalCount = await filteredDocumentTypes.CountAsync();

            return new PagedResultDto<GetDocumentTypeForViewDto>(totalCount, documentTypes.ToList());
        }

        public async Task<GetDocumentTypeForViewDto> GetDocumentTypeForView(long id)
        {
            var documentType = await _documentTypeRepository.GetAsync(id);

            var output = new GetDocumentTypeForViewDto { DocumentType = ObjectMapper.Map<DocumentTypeDto>(documentType) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentTypes_Edit)]
        public async Task<GetDocumentTypeForEditOutput> GetDocumentTypeForEdit(EntityDto<long> input)
        {
            var documentType = await _documentTypeRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetDocumentTypeForEditOutput { DocumentType = ObjectMapper.Map<CreateOrEditDocumentTypeDto>(documentType) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditDocumentTypeDto input)
        {
            var IsDuplicateDocumentType = await _documentTypeRepository.FirstOrDefaultAsync(x => (x.DisplayName).Trim().ToLower() == (input.DisplayName).Trim().ToLower()
            && x.Id != input.Id);
            if (IsDuplicateDocumentType != null)
            {
                throw new UserFriendlyException(string.Format(L("DuplicateDocumentTypeName"), input.DisplayName));
            }

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



            await _documentTypeRepository.InsertAsync(documentType);
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentTypes_Edit)]
        protected virtual async Task Update(CreateOrEditDocumentTypeDto input)
        {
            var documentType = await _documentTypeRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, documentType);
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentTypes_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _documentTypeRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetDocumentTypesToExcel(GetAllDocumentTypesForExcelInput input)
        {

            var filteredDocumentTypes = _documentTypeRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter) || e.DocumentsEntityFk.DisplayName.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter)
                        .WhereIf(input.IsRequiredFilter > -1, e => (input.IsRequiredFilter == 1 && e.IsRequired) || (input.IsRequiredFilter == 0 && !e.IsRequired))
                        .WhereIf(input.HasExpirationDateFilter > -1, e => (input.HasExpirationDateFilter == 1 && e.HasExpirationDate) || (input.HasExpirationDateFilter == 0 && !e.HasExpirationDate))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RequiredFromFilter), e => e.DocumentsEntityFk.DisplayName == input.RequiredFromFilter);

            var query = (from o in filteredDocumentTypes
                         select new GetDocumentTypeForViewDto()
                         {
                             DocumentType = new DocumentTypeDto
                             {
                                 DisplayName = o.DisplayName,
                                 IsRequired = o.IsRequired,
                                 HasExpirationDate = o.HasExpirationDate,
                                 RequiredFrom = o.DocumentsEntityFk.DisplayName,
                                 Id = o.Id
                             }
                         });


            var documentTypeListDtos = await query.ToListAsync();

            return _documentTypesExcelExporter.ExportToFile(documentTypeListDtos);
        }

        public async Task<List<SelectItemDto>> GetAllDocumentsEntitiesForTableDropdown()
        {
            return await _documentsEntityRepository.GetAll()
                .Select(x => new SelectItemDto
                {
                    Id = x.Id.ToString(),
                    DisplayName = x == null || x.DisplayName == null ? "" : x.DisplayName.ToString()
                }).ToListAsync();

        }

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




        public async Task<List<SelectItemDto>> GetDocumentEntitiesForTableDropdown()
        {
            var editions = await _editionRepository.GetAll()
                .Select(x => new SelectItemDto
                {
                    Id = x.Id.ToString(),
                    DisplayName = x.DisplayName
                }).ToListAsync();

            var entities = await _documentEntityRepository
                .GetAll()
                .Where(x => x.Id != 1)
                .Select(x => new SelectItemDto
                {
                    DisplayName = x.DisplayName,
                    Id = x.Id.ToString()
                }
            ).ToListAsync();

            return entities.Concat(editions).ToList();
        }

        public bool IsDocuemntTypeNameAvaliable(string documentTypeName, int? id)
        {
            var result = _documentTypeRepository.FirstOrDefault(x => (x.DisplayName).Trim().ToLower() == (documentTypeName).Trim().ToLower()
            && x.Id != id);

            if (result == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}