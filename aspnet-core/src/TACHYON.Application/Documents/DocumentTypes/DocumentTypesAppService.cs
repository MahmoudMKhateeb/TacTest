

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
using TACHYON.Authorization;
using TACHYON.Documents.DocumentTypes.Dtos;
using TACHYON.Documents.DocumentTypes.Exporting;
using TACHYON.Dto;

namespace TACHYON.Documents.DocumentTypes
{
    [AbpAuthorize(AppPermissions.Pages_DocumentTypes)]
    public class DocumentTypesAppService : TACHYONAppServiceBase, IDocumentTypesAppService
    {
        private readonly IRepository<DocumentType, long> _documentTypeRepository;
        private readonly IDocumentTypesExcelExporter _documentTypesExcelExporter;


        public DocumentTypesAppService(IRepository<DocumentType, long> documentTypeRepository, IDocumentTypesExcelExporter documentTypesExcelExporter)
        {
            _documentTypeRepository = documentTypeRepository;
            _documentTypesExcelExporter = documentTypesExcelExporter;

        }

        public async Task<PagedResultDto<GetDocumentTypeForViewDto>> GetAll(GetAllDocumentTypesInput input)
        {

            var filteredDocumentTypes = _documentTypeRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter) || e.RequiredFrom.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter)
                        .WhereIf(input.IsRequiredFilter > -1, e => (input.IsRequiredFilter == 1 && e.IsRequired) || (input.IsRequiredFilter == 0 && !e.IsRequired))
                        .WhereIf(input.MinExpirationDateFilter != null, e => e.ExpirationDate >= input.MinExpirationDateFilter)
                        .WhereIf(input.MaxExpirationDateFilter != null, e => e.ExpirationDate <= input.MaxExpirationDateFilter)
                        .WhereIf(input.HasExpirationDateFilter > -1, e => (input.HasExpirationDateFilter == 1 && e.HasExpirationDate) || (input.HasExpirationDateFilter == 0 && !e.HasExpirationDate))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RequiredFromFilter), e => e.RequiredFrom == input.RequiredFromFilter);

            var pagedAndFilteredDocumentTypes = filteredDocumentTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var documentTypes = from o in pagedAndFilteredDocumentTypes
                                select new GetDocumentTypeForViewDto()
                                {
                                    DocumentType = new DocumentTypeDto
                                    {
                                        DisplayName = o.DisplayName,
                                        IsRequired = o.IsRequired,
                                        ExpirationDate = o.ExpirationDate,
                                        HasExpirationDate = o.HasExpirationDate,
                                        RequiredFrom = o.RequiredFrom,
                                        Id = o.Id
                                    }
                                };

            var totalCount = await filteredDocumentTypes.CountAsync();

            return new PagedResultDto<GetDocumentTypeForViewDto>(
                totalCount,
                await documentTypes.ToListAsync()
            );
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
            var IsDuplicateDocumentType = await _documentTypeRepository.FirstOrDefaultAsync(x=>(x.DisplayName).Trim().ToLower() == (input.DisplayName).Trim().ToLower());
            if (IsDuplicateDocumentType !=null)
            {
                throw new UserFriendlyException(string.Format(L("DuplicateDocumentTypeName"),input.DisplayName));
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
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter) || e.RequiredFrom.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter)
                        .WhereIf(input.IsRequiredFilter > -1, e => (input.IsRequiredFilter == 1 && e.IsRequired) || (input.IsRequiredFilter == 0 && !e.IsRequired))
                        .WhereIf(input.MinExpirationDateFilter != null, e => e.ExpirationDate >= input.MinExpirationDateFilter)
                        .WhereIf(input.MaxExpirationDateFilter != null, e => e.ExpirationDate <= input.MaxExpirationDateFilter)
                        .WhereIf(input.HasExpirationDateFilter > -1, e => (input.HasExpirationDateFilter == 1 && e.HasExpirationDate) || (input.HasExpirationDateFilter == 0 && !e.HasExpirationDate))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RequiredFromFilter), e => e.RequiredFrom == input.RequiredFromFilter);

            var query = (from o in filteredDocumentTypes
                         select new GetDocumentTypeForViewDto()
                         {
                             DocumentType = new DocumentTypeDto
                             {
                                 DisplayName = o.DisplayName,
                                 IsRequired = o.IsRequired,
                                 ExpirationDate = o.ExpirationDate,
                                 HasExpirationDate = o.HasExpirationDate,
                                 RequiredFrom = o.RequiredFrom,
                                 Id = o.Id
                             }
                         });


            var documentTypeListDtos = await query.ToListAsync();

            return _documentTypesExcelExporter.ExportToFile(documentTypeListDtos);
        }


    }
}