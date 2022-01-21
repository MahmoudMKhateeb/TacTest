using TACHYON.Trucks.TruckCategories.TruckCapacities;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Trucks.TruckCategories.TruckCapacities.TruckCapacitiesTranslations.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Trucks.TruckCategories.TruckCapacities.TruckCapacitiesTranslations
{
    [AbpAuthorize(AppPermissions.Pages_TruckCapacitiesTranslations)]
    public class TruckCapacitiesTranslationsAppService : TACHYONAppServiceBase, ITruckCapacitiesTranslationsAppService
    {
        private readonly IRepository<TruckCapacitiesTranslation> _truckCapacitiesTranslationRepository;
        private readonly IRepository<Capacity, int> _lookup_capacityRepository;

        public TruckCapacitiesTranslationsAppService(
            IRepository<TruckCapacitiesTranslation> truckCapacitiesTranslationRepository,
            IRepository<Capacity, int> lookup_capacityRepository)
        {
            _truckCapacitiesTranslationRepository = truckCapacitiesTranslationRepository;
            _lookup_capacityRepository = lookup_capacityRepository;
        }

        public async Task<PagedResultDto<GetTruckCapacitiesTranslationForViewDto>> GetAll(
            GetAllTruckCapacitiesTranslationsInput input)
        {
            var filteredTruckCapacitiesTranslations = _truckCapacitiesTranslationRepository.GetAll()
                .Include(e => e.Core)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    e => false || e.TranslatedDisplayName.Contains(input.Filter) || e.Language.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.TranslatedDisplayNameFilter),
                    e => e.TranslatedDisplayName == input.TranslatedDisplayNameFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.LanguageFilter), e => e.Language == input.LanguageFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.CapacityDisplayNameFilter),
                    e => e.Core != null && e.Core.DisplayName == input.CapacityDisplayNameFilter);

            var pagedAndFilteredTruckCapacitiesTranslations = filteredTruckCapacitiesTranslations
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var truckCapacitiesTranslations = from o in pagedAndFilteredTruckCapacitiesTranslations
                join o1 in _lookup_capacityRepository.GetAll() on o.CoreId equals o1.Id into j1
                from s1 in j1.DefaultIfEmpty()
                select new GetTruckCapacitiesTranslationForViewDto()
                {
                    TruckCapacitiesTranslation = new TruckCapacitiesTranslationDto
                    {
                        TranslatedDisplayName = o.TranslatedDisplayName, Language = o.Language, Id = o.Id
                    },
                    CapacityDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
                };

            var totalCount = await filteredTruckCapacitiesTranslations.CountAsync();

            return new PagedResultDto<GetTruckCapacitiesTranslationForViewDto>(
                totalCount,
                await truckCapacitiesTranslations.ToListAsync()
            );
        }

        public async Task<GetTruckCapacitiesTranslationForViewDto> GetTruckCapacitiesTranslationForView(int id)
        {
            var truckCapacitiesTranslation = await _truckCapacitiesTranslationRepository.GetAsync(id);

            var output = new GetTruckCapacitiesTranslationForViewDto
            {
                TruckCapacitiesTranslation =
                    ObjectMapper.Map<TruckCapacitiesTranslationDto>(truckCapacitiesTranslation)
            };

            if (output.TruckCapacitiesTranslation.CoreId != null)
            {
                var _lookupCapacity =
                    await _lookup_capacityRepository.FirstOrDefaultAsync((int)output.TruckCapacitiesTranslation.CoreId);
                output.CapacityDisplayName = _lookupCapacity?.DisplayName?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_TruckCapacitiesTranslations_Edit)]
        public async Task<GetTruckCapacitiesTranslationForEditOutput> GetTruckCapacitiesTranslationForEdit(
            EntityDto input)
        {
            var truckCapacitiesTranslation = await _truckCapacitiesTranslationRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTruckCapacitiesTranslationForEditOutput
            {
                TruckCapacitiesTranslation =
                    ObjectMapper.Map<CreateOrEditTruckCapacitiesTranslationDto>(truckCapacitiesTranslation)
            };

            if (output.TruckCapacitiesTranslation.CoreId != null)
            {
                var _lookupCapacity =
                    await _lookup_capacityRepository.FirstOrDefaultAsync((int)output.TruckCapacitiesTranslation.CoreId);
                output.CapacityDisplayName = _lookupCapacity?.DisplayName?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTruckCapacitiesTranslationDto input)
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

        [AbpAuthorize(AppPermissions.Pages_TruckCapacitiesTranslations_Create)]
        protected virtual async Task Create(CreateOrEditTruckCapacitiesTranslationDto input)
        {
            var truckCapacitiesTranslation = ObjectMapper.Map<TruckCapacitiesTranslation>(input);

            await _truckCapacitiesTranslationRepository.InsertAsync(truckCapacitiesTranslation);
        }

        [AbpAuthorize(AppPermissions.Pages_TruckCapacitiesTranslations_Edit)]
        protected virtual async Task Update(CreateOrEditTruckCapacitiesTranslationDto input)
        {
            var truckCapacitiesTranslation =
                await _truckCapacitiesTranslationRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, truckCapacitiesTranslation);
        }

        [AbpAuthorize(AppPermissions.Pages_TruckCapacitiesTranslations_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _truckCapacitiesTranslationRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_TruckCapacitiesTranslations)]
        public async Task<List<TruckCapacitiesTranslationCapacityLookupTableDto>> GetAllCapacityForTableDropdown()
        {
            return await _lookup_capacityRepository.GetAll()
                .Select(capacity => new TruckCapacitiesTranslationCapacityLookupTableDto
                {
                    Id = capacity.Id,
                    DisplayName = capacity == null || capacity.DisplayName == null
                        ? ""
                        : capacity.DisplayName.ToString()
                }).ToListAsync();
        }
    }
}