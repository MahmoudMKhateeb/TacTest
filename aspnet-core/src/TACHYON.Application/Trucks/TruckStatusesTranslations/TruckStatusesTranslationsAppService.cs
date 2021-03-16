using TACHYON.Trucks;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Trucks.TruckStatusesTranslations.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Trucks.TruckStatusesTranslations
{
    [AbpAuthorize(AppPermissions.Pages_TruckStatusesTranslations)]
    public class TruckStatusesTranslationsAppService : TACHYONAppServiceBase, ITruckStatusesTranslationsAppService
    {
        private readonly IRepository<TruckStatusesTranslation> _truckStatusesTranslationRepository;
        private readonly IRepository<TruckStatus, long> _lookup_truckStatusRepository;

        public TruckStatusesTranslationsAppService(IRepository<TruckStatusesTranslation> truckStatusesTranslationRepository, IRepository<TruckStatus, long> lookup_truckStatusRepository)
        {
            _truckStatusesTranslationRepository = truckStatusesTranslationRepository;
            _lookup_truckStatusRepository = lookup_truckStatusRepository;

        }

        public async Task<PagedResultDto<GetTruckStatusesTranslationForViewDto>> GetAll(GetAllTruckStatusesTranslationsInput input)
        {

            var filteredTruckStatusesTranslations = _truckStatusesTranslationRepository.GetAll()
                        .Include(e => e.Core)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.TranslatedDisplayName.Contains(input.Filter) || e.Language.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TranslatedDisplayNameFilter), e => e.TranslatedDisplayName == input.TranslatedDisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LanguageFilter), e => e.Language == input.LanguageFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TruckStatusDisplayNameFilter), e => e.Core != null && e.Core.DisplayName == input.TruckStatusDisplayNameFilter);

            var pagedAndFilteredTruckStatusesTranslations = filteredTruckStatusesTranslations
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var truckStatusesTranslations = from o in pagedAndFilteredTruckStatusesTranslations
                                            join o1 in _lookup_truckStatusRepository.GetAll() on o.CoreId equals o1.Id into j1
                                            from s1 in j1.DefaultIfEmpty()

                                            select new GetTruckStatusesTranslationForViewDto()
                                            {
                                                TruckStatusesTranslation = new TruckStatusesTranslationDto
                                                {
                                                    TranslatedDisplayName = o.TranslatedDisplayName,
                                                    Language = o.Language,
                                                    Id = o.Id
                                                },
                                                TruckStatusDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
                                            };

            var totalCount = await filteredTruckStatusesTranslations.CountAsync();

            return new PagedResultDto<GetTruckStatusesTranslationForViewDto>(
                totalCount,
                await truckStatusesTranslations.ToListAsync()
            );
        }

        public async Task<GetTruckStatusesTranslationForViewDto> GetTruckStatusesTranslationForView(int id)
        {
            var truckStatusesTranslation = await _truckStatusesTranslationRepository.GetAsync(id);

            var output = new GetTruckStatusesTranslationForViewDto { TruckStatusesTranslation = ObjectMapper.Map<TruckStatusesTranslationDto>(truckStatusesTranslation) };

            if (output.TruckStatusesTranslation.CoreId != null)
            {
                var _lookupTruckStatus = await _lookup_truckStatusRepository.FirstOrDefaultAsync((long)output.TruckStatusesTranslation.CoreId);
                output.TruckStatusDisplayName = _lookupTruckStatus?.DisplayName?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_TruckStatusesTranslations_Edit)]
        public async Task<GetTruckStatusesTranslationForEditOutput> GetTruckStatusesTranslationForEdit(EntityDto input)
        {
            var truckStatusesTranslation = await _truckStatusesTranslationRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTruckStatusesTranslationForEditOutput { TruckStatusesTranslation = ObjectMapper.Map<CreateOrEditTruckStatusesTranslationDto>(truckStatusesTranslation) };

            if (output.TruckStatusesTranslation.CoreId != null)
            {
                var _lookupTruckStatus = await _lookup_truckStatusRepository.FirstOrDefaultAsync((long)output.TruckStatusesTranslation.CoreId);
                output.TruckStatusDisplayName = _lookupTruckStatus?.DisplayName?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTruckStatusesTranslationDto input)
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

        [AbpAuthorize(AppPermissions.Pages_TruckStatusesTranslations_Create)]
        protected virtual async Task Create(CreateOrEditTruckStatusesTranslationDto input)
        {
            var truckStatusesTranslation = ObjectMapper.Map<TruckStatusesTranslation>(input);

            await _truckStatusesTranslationRepository.InsertAsync(truckStatusesTranslation);
        }

        [AbpAuthorize(AppPermissions.Pages_TruckStatusesTranslations_Edit)]
        protected virtual async Task Update(CreateOrEditTruckStatusesTranslationDto input)
        {
            var truckStatusesTranslation = await _truckStatusesTranslationRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, truckStatusesTranslation);
        }

        [AbpAuthorize(AppPermissions.Pages_TruckStatusesTranslations_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _truckStatusesTranslationRepository.DeleteAsync(input.Id);
        }
        [AbpAuthorize(AppPermissions.Pages_TruckStatusesTranslations)]
        public async Task<List<TruckStatusesTranslationTruckStatusLookupTableDto>> GetAllTruckStatusForTableDropdown()
        {
            return await _lookup_truckStatusRepository.GetAll()
                .Select(truckStatus => new TruckStatusesTranslationTruckStatusLookupTableDto
                {
                    Id = truckStatus.Id,
                    DisplayName = truckStatus == null || truckStatus.DisplayName == null ? "" : truckStatus.DisplayName.ToString()
                }).ToListAsync();
        }

    }
}