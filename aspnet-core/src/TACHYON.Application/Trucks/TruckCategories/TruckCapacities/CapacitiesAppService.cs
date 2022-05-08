using System.Collections.Generic;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Trucks.TruckCategories.TruckCapacities.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using TACHYON.Trucks.TruckCategories.TransportTypes.Dtos;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Trucks.TruckCategories.TruckCapacities
{
    [AbpAuthorize(AppPermissions.Pages_Capacities)]
    public class CapacitiesAppService : TACHYONAppServiceBase, ICapacitiesAppService
    {
        private readonly IRepository<Capacity> _capacityRepository;
        private readonly IRepository<TrucksType, long> _lookup_trucktypeRepository;


        public CapacitiesAppService(IRepository<Capacity> capacityRepository,
            IRepository<TrucksType, long> lookup_truckTypeRepository)
        {
            _capacityRepository = capacityRepository;
            _lookup_trucktypeRepository = lookup_truckTypeRepository;
        }

        public async Task<PagedResultDto<GetCapacityForViewDto>> GetAll(GetAllCapacitiesInput input)
        {
            var filteredCapacities = _capacityRepository.GetAll()
                .Include(e => e.TrucksTypeFk)
                .ThenInclude(x => x.Translations)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),
                    e => e.DisplayName == input.DisplayNameFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.TruckTypeDisplayNameFilter),
                    e => e.TrucksTypeFk != null && e.Translations.Any(x =>
                        x.TranslatedDisplayName.Contains(input.TruckTypeDisplayNameFilter)));

            var pagedAndFilteredCapacities = filteredCapacities
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var capacities = from o in await pagedAndFilteredCapacities.ToListAsync()
                //join o1 in _lookup_trucktypeRepository.GetAll() on o.TrucksTypeId equals o1.Id into j1
                //from s1 in j1.DefaultIfEmpty()
                select new GetCapacityForViewDto()
                {
                    Capacity = new CapacityDto { DisplayName = o.DisplayName, Id = o.Id },
                    TruckTypeDisplayName =
                        ObjectMapper.Map<TrucksTypeDto>(o.TrucksTypeFk)
                            ?.TranslatedDisplayName //s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
                };

            var totalCount = await filteredCapacities.CountAsync();

            return new PagedResultDto<GetCapacityForViewDto>(
                totalCount,
                capacities.ToList()
            );
        }

        public async Task<GetCapacityForViewDto> GetCapacityForView(int id)
        {
            var capacity = await _capacityRepository.GetAsync(id);

            var output = new GetCapacityForViewDto { Capacity = ObjectMapper.Map<CapacityDto>(capacity) };

            if (output.Capacity.TrucksTypeId != null)
            {
                var _lookupTruckType = await _lookup_trucktypeRepository.GetAllIncluding(x => x.Translations)
                    .FirstOrDefaultAsync(x => x.Id == (int)output.Capacity.TrucksTypeId);
                output.TruckTypeDisplayName = _lookupTruckType != null
                    ? ObjectMapper.Map<TrucksTypeDto>(_lookupTruckType).TranslatedDisplayName
                    : ""; //_lookupTruckType?.DisplayName?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Capacities_Edit)]
        public async Task<GetCapacityForEditOutput> GetCapacityForEdit(EntityDto input)
        {
            var capacity = await _capacityRepository.FirstOrDefaultAsync(input.Id);

            var output =
                new GetCapacityForEditOutput { Capacity = ObjectMapper.Map<CreateOrEditCapacityDto>(capacity) };

            if (output.Capacity.TrucksTypeId != null)
            {
                var _lookupTruckType = await _lookup_trucktypeRepository.GetAllIncluding(x => x.Translations)
                    .FirstOrDefaultAsync(x => x.Id == (int)output.Capacity.TrucksTypeId);
                output.TruckTypeDisplayName =
                    ObjectMapper.Map<TrucksTypeDto>(_lookupTruckType)
                        ?.TranslatedDisplayName; //_lookupTruckType?.DisplayName?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditCapacityDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Capacities_Create)]
        protected virtual async Task Create(CreateOrEditCapacityDto input)
        {
            var capacity = ObjectMapper.Map<Capacity>(input);


            await _capacityRepository.InsertAsync(capacity);
        }

        [AbpAuthorize(AppPermissions.Pages_Capacities_Edit)]
        protected virtual async Task Update(CreateOrEditCapacityDto input)
        {
            var capacity = await _capacityRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, capacity);
        }

        [AbpAuthorize(AppPermissions.Pages_Capacities_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _capacityRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_Capacities)]
        public async Task<IEnumerable<ISelectItemDto>> GetAllTruckTypeForTableDropdown()
        {
            List<TrucksType> trucksTypes = await _lookup_trucktypeRepository
                .GetAllIncluding(x => x.Translations)
                .ToListAsync();

            List<TrucksTypeSelectItemDto> trucksTypeDtos = ObjectMapper.Map<List<TrucksTypeSelectItemDto>>(trucksTypes);

            return trucksTypeDtos;
        }
    }
}