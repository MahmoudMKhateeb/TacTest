using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Documents.DocumentsEntities.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Documents.DocumentsEntities
{
    [AbpAuthorize(AppPermissions.Pages_DocumentsEntities)]
    public class DocumentsEntitiesAppService : TACHYONAppServiceBase, IDocumentsEntitiesAppService
    {
        private readonly IRepository<DocumentsEntity> _documentsEntityRepository;


        public DocumentsEntitiesAppService(IRepository<DocumentsEntity> documentsEntityRepository)
        {
            _documentsEntityRepository = documentsEntityRepository;
        }

        public async Task<PagedResultDto<GetDocumentsEntityForViewDto>> GetAll(GetAllDocumentsEntitiesInput input)
        {
            var filteredDocumentsEntities = _documentsEntityRepository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),
                    e => e.DisplayName == input.DisplayNameFilter);

            var pagedAndFilteredDocumentsEntities = filteredDocumentsEntities
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var documentsEntities = from o in pagedAndFilteredDocumentsEntities
                select new GetDocumentsEntityForViewDto()
                {
                    DocumentsEntity = new DocumentsEntityDto { DisplayName = o.DisplayName, Id = o.Id }
                };

            var totalCount = await filteredDocumentsEntities.CountAsync();

            return new PagedResultDto<GetDocumentsEntityForViewDto>(
                totalCount,
                await documentsEntities.ToListAsync()
            );
        }

        public async Task<GetDocumentsEntityForViewDto> GetDocumentsEntityForView(int id)
        {
            var documentsEntity = await _documentsEntityRepository.GetAsync(id);

            var output = new GetDocumentsEntityForViewDto
            {
                DocumentsEntity = ObjectMapper.Map<DocumentsEntityDto>(documentsEntity)
            };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentsEntities_Edit)]
        public async Task<GetDocumentsEntityForEditOutput> GetDocumentsEntityForEdit(EntityDto input)
        {
            var documentsEntity = await _documentsEntityRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetDocumentsEntityForEditOutput
            {
                DocumentsEntity = ObjectMapper.Map<CreateOrEditDocumentsEntityDto>(documentsEntity)
            };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditDocumentsEntityDto input)
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

        [AbpAuthorize(AppPermissions.Pages_DocumentsEntities_Create)]
        protected virtual async Task Create(CreateOrEditDocumentsEntityDto input)
        {
            var documentsEntity = ObjectMapper.Map<DocumentsEntity>(input);


            await _documentsEntityRepository.InsertAsync(documentsEntity);
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentsEntities_Edit)]
        protected virtual async Task Update(CreateOrEditDocumentsEntityDto input)
        {
            var documentsEntity = await _documentsEntityRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, documentsEntity);
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentsEntities_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _documentsEntityRepository.DeleteAsync(input.Id);
        }
    }
}