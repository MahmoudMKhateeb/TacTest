using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;

namespace TACHYON.EntityTemplates
{
    [AbpAuthorize(AppPermissions.Pages_EntityTemplate)]
    public class EntityTemplateAppService : TACHYONAppServiceBase
    {
        private readonly EntityTemplateManager _templateManager;
        private readonly IRepository<EntityTemplate, long> _templateRepository;

        public EntityTemplateAppService(
            EntityTemplateManager templateManager,
            IRepository<EntityTemplate, long> templateRepository)
        {
            _templateManager = templateManager;
            _templateRepository = templateRepository;
        }

        public async Task<PagedResultDto<EntityTemplateListDto>> GetAll(GetEntityTemplateInputDto input)
        {
            var templates =  _templateRepository.GetAll().AsNoTracking()
                .Where(x => x.EntityType == input.Type)
                .WhereIf(!input.Filter.IsNullOrEmpty(), x => x.SavedEntityId.Contains(input.Filter))
                .OrderBy(input.Sorting??"Id desc");

            var totalCount = await templates.CountAsync();

            var pageResult = await templates.PageBy(input).ToListAsync();

            return new PagedResultDto<EntityTemplateListDto>()
            {
                Items = ObjectMapper.Map<List<EntityTemplateListDto>>(pageResult), TotalCount = totalCount
            };
        }

        public async Task CreateOrEdit(CreateOrEditEntityTemplateInputDto input)
        {
            if (input.Id.HasValue)
            {
                await Update(input);
                return;
            }

            await Create(input);
        }

        public async Task<EntityTemplateForViewDto> GetForView(EntityDto<long> input)
        {
            var template = await _templateRepository.GetAll().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (template == null)
                throw new EntityNotFoundException(L("TemplateNotFound"));

            return ObjectMapper.Map<EntityTemplateForViewDto>(template);
        }
        [AbpAuthorize(AppPermissions.Pages_EntityTemplate_Create)]
        protected virtual async Task Create(CreateOrEditEntityTemplateInputDto input)
        {
            await _templateManager.Create(input);
        }
        [AbpAuthorize(AppPermissions.Pages_EntityTemplate_Update)]
        protected virtual async Task Update(CreateOrEditEntityTemplateInputDto input)
        {
            await _templateManager.Update(input);
        }
        
    }
}