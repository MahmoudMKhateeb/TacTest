using TACHYON.Documents.DocumentTypes;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Documents.DocumentTypeTranslations.Exporting;
using TACHYON.Documents.DocumentTypeTranslations.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Documents.DocumentTypeTranslations
{
    [AbpAuthorize(AppPermissions.Pages_DocumentTypeTranslations)]
    public class DocumentTypeTranslationsAppService : TACHYONAppServiceBase, IDocumentTypeTranslationsAppService
    {
        private readonly IRepository<DocumentTypeTranslation> _documentTypeTranslationRepository;
        private readonly IDocumentTypeTranslationsExcelExporter _documentTypeTranslationsExcelExporter;
        private readonly IRepository<DocumentType, long> _lookup_documentTypeRepository;


        public DocumentTypeTranslationsAppService(
            IRepository<DocumentTypeTranslation> documentTypeTranslationRepository,
            IDocumentTypeTranslationsExcelExporter documentTypeTranslationsExcelExporter,
            IRepository<DocumentType, long> lookup_documentTypeRepository)
        {
            _documentTypeTranslationRepository = documentTypeTranslationRepository;
            _documentTypeTranslationsExcelExporter = documentTypeTranslationsExcelExporter;
            _lookup_documentTypeRepository = lookup_documentTypeRepository;
        }

        public async Task<PagedResultDto<GetDocumentTypeTranslationForViewDto>> GetAll(
            GetAllDocumentTypeTranslationsInput input)
        {
            var filteredDocumentTypeTranslations = _documentTypeTranslationRepository.GetAll()
                .Include(e => e.Core)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    e => false || e.Name.Contains(input.Filter) || e.Language.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.LanguageFilter), e => e.Language == input.LanguageFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.DocumentTypeDisplayNameFilter),
                    e => e.Core != null && e.Core.DisplayName == input.DocumentTypeDisplayNameFilter);

            var pagedAndFilteredDocumentTypeTranslations = filteredDocumentTypeTranslations
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var documentTypeTranslations = from o in pagedAndFilteredDocumentTypeTranslations
                join o1 in _lookup_documentTypeRepository.GetAll() on o.CoreId equals o1.Id into j1
                from s1 in j1.DefaultIfEmpty()
                select new GetDocumentTypeTranslationForViewDto()
                {
                    DocumentTypeTranslation =
                        new DocumentTypeTranslationDto { Name = o.Name, Language = o.Language, Id = o.Id },
                    DocumentTypeDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
                };

            var totalCount = await filteredDocumentTypeTranslations.CountAsync();

            return new PagedResultDto<GetDocumentTypeTranslationForViewDto>(
                totalCount,
                await documentTypeTranslations.ToListAsync()
            );
        }

        public async Task<GetDocumentTypeTranslationForViewDto> GetDocumentTypeTranslationForView(int id)
        {
            var documentTypeTranslation = await _documentTypeTranslationRepository.GetAsync(id);

            var output = new GetDocumentTypeTranslationForViewDto
            {
                DocumentTypeTranslation = ObjectMapper.Map<DocumentTypeTranslationDto>(documentTypeTranslation)
            };

            if (output.DocumentTypeTranslation.CoreId != null)
            {
                var _lookupDocumentType =
                    await _lookup_documentTypeRepository.FirstOrDefaultAsync(
                        (long)output.DocumentTypeTranslation.CoreId);
                output.DocumentTypeDisplayName = _lookupDocumentType?.DisplayName?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentTypeTranslations_Edit)]
        public async Task<GetDocumentTypeTranslationForEditOutput> GetDocumentTypeTranslationForEdit(EntityDto input)
        {
            var documentTypeTranslation = await _documentTypeTranslationRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetDocumentTypeTranslationForEditOutput
            {
                DocumentTypeTranslation =
                    ObjectMapper.Map<CreateOrEditDocumentTypeTranslationDto>(documentTypeTranslation)
            };

            if (output.DocumentTypeTranslation.CoreId != null)
            {
                var _lookupDocumentType =
                    await _lookup_documentTypeRepository.FirstOrDefaultAsync(
                        (long)output.DocumentTypeTranslation.CoreId);
                output.DocumentTypeDisplayName = _lookupDocumentType?.DisplayName?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditDocumentTypeTranslationDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentTypeTranslations_Create)]
        protected virtual async Task Create(CreateOrEditDocumentTypeTranslationDto input)
        {
            var documentTypeTranslation = ObjectMapper.Map<DocumentTypeTranslation>(input);


            await _documentTypeTranslationRepository.InsertAsync(documentTypeTranslation);
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentTypeTranslations_Edit)]
        protected virtual async Task Update(CreateOrEditDocumentTypeTranslationDto input)
        {
            var documentTypeTranslation = await _documentTypeTranslationRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, documentTypeTranslation);
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentTypeTranslations_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _documentTypeTranslationRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetDocumentTypeTranslationsToExcel(GetAllDocumentTypeTranslationsForExcelInput input)
        {
            var filteredDocumentTypeTranslations = _documentTypeTranslationRepository.GetAll()
                .Include(e => e.Core)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    e => false || e.Name.Contains(input.Filter) || e.Language.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.LanguageFilter), e => e.Language == input.LanguageFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.DocumentTypeDisplayNameFilter),
                    e => e.Core != null && e.Core.DisplayName == input.DocumentTypeDisplayNameFilter);

            var query = (from o in filteredDocumentTypeTranslations
                join o1 in _lookup_documentTypeRepository.GetAll() on o.CoreId equals o1.Id into j1
                from s1 in j1.DefaultIfEmpty()
                select new GetDocumentTypeTranslationForViewDto()
                {
                    DocumentTypeTranslation =
                        new DocumentTypeTranslationDto { Name = o.Name, Language = o.Language, Id = o.Id },
                    DocumentTypeDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
                });


            var documentTypeTranslationListDtos = await query.ToListAsync();

            return _documentTypeTranslationsExcelExporter.ExportToFile(documentTypeTranslationListDtos);
        }


        [AbpAuthorize(AppPermissions.Pages_DocumentTypeTranslations)]
        public async Task<List<DocumentTypeTranslationDocumentTypeLookupTableDto>> GetAllDocumentTypeForTableDropdown()
        {
            return await _lookup_documentTypeRepository.GetAll()
                .Select(documentType => new DocumentTypeTranslationDocumentTypeLookupTableDto
                {
                    Id = documentType.Id,
                    DisplayName = documentType == null || documentType.DisplayName == null
                        ? ""
                        : documentType.DisplayName.ToString()
                }).ToListAsync();
        }
    }
}