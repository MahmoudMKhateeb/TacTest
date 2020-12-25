

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.TermsAndConditions.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.Application.Editions;
using Abp.UI;

namespace TACHYON.TermsAndConditions
{
    [AbpAuthorize(AppPermissions.Pages_TermAndConditions)]
    public class TermAndConditionsAppService : TACHYONAppServiceBase, ITermAndConditionsAppService
    {
        private readonly IRepository<TermAndCondition> _termAndConditionRepository;
        private readonly IRepository<Edition, int> _editionRepository;


        public TermAndConditionsAppService(IRepository<TermAndCondition> termAndConditionRepository, IRepository<Edition, int> editionRepository)
        {
            _termAndConditionRepository = termAndConditionRepository;
            _editionRepository = editionRepository;
        }

        public async Task<PagedResultDto<GetTermAndConditionForViewDto>> GetAll(GetAllTermAndConditionsInput input)
        {
            var filteredTermAndConditions = _termAndConditionRepository.GetAll()
                .Include(x => x.EditionFk)
                .Include(x => x.Translations)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Title.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.TitleFilter), e => e.Title == input.TitleFilter)
                .WhereIf(input.MinVersionFilter != null, e => e.Version >= input.MinVersionFilter)
                .WhereIf(input.MaxVersionFilter != null, e => e.Version <= input.MaxVersionFilter);

            var pagedAndFilteredTermAndConditions = filteredTermAndConditions
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var termAndConditions = pagedAndFilteredTermAndConditions
                .Select(x => new GetTermAndConditionForViewDto { TermAndCondition = ObjectMapper.Map<TermAndConditionDto>(x) });

            var totalCount = await filteredTermAndConditions.CountAsync();
            return new PagedResultDto<GetTermAndConditionForViewDto>(
                totalCount,
               await termAndConditions.ToListAsync()
            );
        }

        public async Task<GetTermAndConditionForViewDto> GetTermAndConditionForView(int id)
        {
            var termAndCondition = await _termAndConditionRepository.GetAll()
                .Include(x => x.EditionFk)
                .Include(x => x.Translations)
                .SingleAsync(x => x.Id == id);

            var output = new GetTermAndConditionForViewDto { TermAndCondition = ObjectMapper.Map<TermAndConditionDto>(termAndCondition) };
            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_TermAndConditions_Edit)]
        public async Task<GetTermAndConditionForEditOutput> GetTermAndConditionForEdit(EntityDto input)
        {
            var termAndCondition = await _termAndConditionRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTermAndConditionForEditOutput { TermAndCondition = ObjectMapper.Map<CreateOrEditTermAndConditionDto>(termAndCondition) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTermAndConditionDto input)
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

        [AbpAuthorize(AppPermissions.Pages_TermAndConditions_Create)]
        protected virtual async Task Create(CreateOrEditTermAndConditionDto input)
        {
            var termAndCondition = ObjectMapper.Map<TermAndCondition>(input);

            await _termAndConditionRepository.InsertAsync(termAndCondition);
        }

        [AbpAuthorize(AppPermissions.Pages_TermAndConditions_Edit)]
        protected virtual async Task Update(CreateOrEditTermAndConditionDto input)
        {
            var termAndCondition = await _termAndConditionRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, termAndCondition);
        }

        [AbpAuthorize(AppPermissions.Pages_TermAndConditions_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _termAndConditionRepository.DeleteAsync(input.Id);
        }

        public async Task<List<SelectItemDto>> GetDocumentEntitiesForTableDropdown()
        {
            var editions = await _editionRepository.GetAll()
                .Select(x => new SelectItemDto
                {
                    Id = x.Id.ToString(),
                    DisplayName = x.DisplayName
                }).ToListAsync();

            return editions.ToList();
        }
        public async Task SetAsActive(int id)
        {
            var term = await _termAndConditionRepository.GetAsync(id);
            if (term == null)
            {
                throw new UserFriendlyException(L("TermAndConditonNotFound"));

            }
            term.IsActive = true;

            var allTerms = await _termAndConditionRepository.GetAll().Where(x => x.Id != id && x.EditionId == term.EditionId).ToListAsync();
            if (allTerms == null)
            {
                return;
            }
            foreach (var item in allTerms)
            {
                item.IsActive = false;

            }
            return;
        }


    }
}