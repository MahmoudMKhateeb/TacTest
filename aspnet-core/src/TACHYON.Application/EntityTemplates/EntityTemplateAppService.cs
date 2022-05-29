using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Validation;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Dto;

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
                .WhereIf(input.Type.HasValue,x => x.EntityType == input.Type)
                .WhereIf(!input.Filter.IsNullOrEmpty(), x => x.TemplateName.Contains(input.Filter))
                .OrderBy(input.Sorting??"Id desc")
                .ProjectTo<EntityTemplateListDto>(AutoMapperConfigurationProvider);

            var totalCount = await templates.CountAsync();

            var pageResult = await templates.PageBy(input).ToListAsync();

            return new PagedResultDto<EntityTemplateListDto>()
            {
                Items = pageResult, TotalCount = totalCount
            };
        }

        public async Task<string> CreateOrEdit(CreateOrEditEntityTemplateInputDto input)
        {
            await CheckDuplicatedTemplateName(input);
            if (input.Id.HasValue)
                return await Update(input);

            return await Create(input);
        }

        public async Task<EntityTemplateForViewDto> GetForView(EntityDto<long> input)
        {
            var template = await _templateRepository.GetAll().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (template == null)
                throw new EntityNotFoundException(L("TemplateNotFound"));

            return ObjectMapper.Map<EntityTemplateForViewDto>(template);
        }

        public async Task<List<SelectItemDto>> GetAllForDropdown(GetAllTemplateForDropdownInputDto input)
        {
            var templates = _templateRepository.GetAll().AsNoTracking().Where(x => x.EntityType == input.Type);

            switch (input.Type)
            {
                case SavedEntityType.ShippingRequestTemplate:
                    return await templates
                        .Select(x => new SelectItemDto() {DisplayName = x.TemplateName, Id = x.Id.ToString()})
                        .ToListAsync();
                case SavedEntityType.TripTemplate:

                    var tripTemplates = await templates.ToListAsync();
                    if (!input.ParentEntityId.IsNullOrEmpty()) 
                        return await _templateManager.FilterTripTemplatesByParentEntity(tripTemplates, input.ParentEntityId);
                    
                    return await templates
                        .Select(x => new SelectItemDto() {DisplayName = x.TemplateName, Id = x.Id.ToString()})
                        .ToListAsync();
                default:
                    throw new UserFriendlyException(L("NotSupportedEntityType"));
            }

           
        }


        [AbpAuthorize(AppPermissions.Pages_EntityTemplate_Create)]
        protected virtual async Task<string> Create(CreateOrEditEntityTemplateInputDto input)
        => await _templateManager.Create(input);
        
        [AbpAuthorize(AppPermissions.Pages_EntityTemplate_Update)]
        protected virtual async Task<string> Update(CreateOrEditEntityTemplateInputDto input) 
            => await _templateManager.Update(input);

        public virtual async Task Delete(EntityDto<long> input)
        {
            var isExist = await _templateRepository.GetAll().AnyAsync(x => x.Id == input.Id);
            if (!isExist)
                throw new EntityNotFoundException(L("TemplateNotFount"));
            
            await _templateRepository.DeleteAsync(x => x.Id == input.Id);
        }
        
        private async Task CheckDuplicatedTemplateName(CreateOrEditEntityTemplateInputDto input)
        {
            
           var isDuplicated = await _templateRepository.GetAll()
               .WhereIf(input.Id.HasValue, x => x.Id != input.Id)
                .AnyAsync(x => x.TemplateName.ToLower().Equals(input.TemplateName.ToLower()));
           if (isDuplicated)
               throw new AbpValidationException(L("TemplateNameAlreadyUsed"));
        }
    }
}