using TACHYON.TermsAndConditions;

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

namespace TACHYON.TermsAndConditions
{
    [AbpAuthorize(AppPermissions.Pages_Administration_TermAndConditionTranslations)]
    public class TermAndConditionTranslationsAppService : TACHYONAppServiceBase, ITermAndConditionTranslationsAppService
    {
        private readonly IRepository<TermAndConditionTranslation> _termAndConditionTranslationRepository;
        private readonly IRepository<TermAndCondition, int> _lookup_termAndConditionRepository;

        public TermAndConditionTranslationsAppService(IRepository<TermAndConditionTranslation> termAndConditionTranslationRepository, IRepository<TermAndCondition, int> lookup_termAndConditionRepository)
        {
            _termAndConditionTranslationRepository = termAndConditionTranslationRepository;
            _lookup_termAndConditionRepository = lookup_termAndConditionRepository;

        }

        public async Task<PagedResultDto<GetTermAndConditionTranslationForViewDto>> GetAll(GetAllTermAndConditionTranslationsInput input)
        {

            var filteredTermAndConditionTranslations = _termAndConditionTranslationRepository.GetAll()
                        .Include(e => e.Core)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Content.Contains(input.Filter) || e.Language.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LanguageFilter), e => e.Language == input.LanguageFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TermAndConditionTitleFilter), e => e.Core != null && e.Core.Title == input.TermAndConditionTitleFilter);

            var pagedAndFilteredTermAndConditionTranslations = filteredTermAndConditionTranslations
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var termAndConditionTranslations = from o in pagedAndFilteredTermAndConditionTranslations
                                               join o1 in _lookup_termAndConditionRepository.GetAll() on o.CoreId equals o1.Id into j1
                                               from s1 in j1.DefaultIfEmpty()

                                               select new GetTermAndConditionTranslationForViewDto()
                                               {
                                                   TermAndConditionTranslation = new TermAndConditionTranslationDto
                                                   {
                                                       Language = o.Language,
                                                       Id = o.Id
                                                   },
                                                   TermAndConditionTitle = s1 == null || s1.Title == null ? "" : s1.Title.ToString()
                                               };

            var totalCount = await filteredTermAndConditionTranslations.CountAsync();

            return new PagedResultDto<GetTermAndConditionTranslationForViewDto>(
                totalCount,
                await termAndConditionTranslations.ToListAsync()
            );
        }

        public async Task<GetTermAndConditionTranslationForViewDto> GetTermAndConditionTranslationForView(int id)
        {
            var termAndConditionTranslation = await _termAndConditionTranslationRepository.GetAsync(id);

            var output = new GetTermAndConditionTranslationForViewDto { TermAndConditionTranslation = ObjectMapper.Map<TermAndConditionTranslationDto>(termAndConditionTranslation) };

            if (output.TermAndConditionTranslation.CoreId != null)
            {
                var _lookupTermAndCondition = await _lookup_termAndConditionRepository.FirstOrDefaultAsync((int)output.TermAndConditionTranslation.CoreId);
                output.TermAndConditionTitle = _lookupTermAndCondition?.Title?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TermAndConditionTranslations_Edit)]
        public async Task<GetTermAndConditionTranslationForEditOutput> GetTermAndConditionTranslationForEdit(EntityDto input)
        {
            var termAndConditionTranslation = await _termAndConditionTranslationRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTermAndConditionTranslationForEditOutput { TermAndConditionTranslation = ObjectMapper.Map<CreateOrEditTermAndConditionTranslationDto>(termAndConditionTranslation) };

            if (output.TermAndConditionTranslation.CoreId != null)
            {
                var _lookupTermAndCondition = await _lookup_termAndConditionRepository.FirstOrDefaultAsync((int)output.TermAndConditionTranslation.CoreId);
                output.TermAndConditionTitle = _lookupTermAndCondition?.Title?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTermAndConditionTranslationDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Administration_TermAndConditionTranslations_Create)]
        protected virtual async Task Create(CreateOrEditTermAndConditionTranslationDto input)
        {
            var termAndConditionTranslation = ObjectMapper.Map<TermAndConditionTranslation>(input);

            await _termAndConditionTranslationRepository.InsertAsync(termAndConditionTranslation);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TermAndConditionTranslations_Edit)]
        protected virtual async Task Update(CreateOrEditTermAndConditionTranslationDto input)
        {
            var termAndConditionTranslation = await _termAndConditionTranslationRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, termAndConditionTranslation);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TermAndConditionTranslations_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _termAndConditionTranslationRepository.DeleteAsync(input.Id);
        }
        [AbpAuthorize(AppPermissions.Pages_Administration_TermAndConditionTranslations)]
        public async Task<List<TermAndConditionTranslationTermAndConditionLookupTableDto>> GetAllTermAndConditionForTableDropdown()
        {
            return await _lookup_termAndConditionRepository.GetAll()
                .Select(termAndCondition => new TermAndConditionTranslationTermAndConditionLookupTableDto
                {
                    Id = termAndCondition.Id,
                    DisplayName = termAndCondition == null || termAndCondition.Title == null ? "" : termAndCondition.Title.ToString()
                }).ToListAsync();
        }

    }
}