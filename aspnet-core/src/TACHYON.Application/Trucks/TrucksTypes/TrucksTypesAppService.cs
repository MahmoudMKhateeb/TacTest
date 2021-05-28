

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
using TACHYON.Dto;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TruckCategories.TransportTypes.Dtos;
using TACHYON.Trucks.TrucksTypes.Dtos;
using TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations;

namespace TACHYON.Trucks.TrucksTypes
{
    [AbpAuthorize(AppPermissions.Pages_TrucksTypes)]
    public class TrucksTypesAppService : TACHYONAppServiceBase, ITrucksTypesAppService
    {
        private readonly IRepository<TrucksType, long> _trucksTypeRepository;

        private readonly IRepository<TransportType, int> _transportTypeRepository;

        public TrucksTypesAppService(IRepository<TrucksType, long> trucksTypeRepository, IRepository<TransportType, int> transportTypeRepository)
        {
            _trucksTypeRepository = trucksTypeRepository;
            _transportTypeRepository = transportTypeRepository;
        }

        public async Task<PagedResultDto<GetTrucksTypeForViewDto>> GetAll(GetAllTrucksTypesInput input)
        {

            var filteredTrucksTypes = _trucksTypeRepository.GetAll()
                .Include(x=>x.Translations)
                .Include(x=>x.TransportTypeFk)
                .ThenInclude(x=>x.Translations)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Translations.Any(x=>x.TranslatedDisplayName.Contains(input.Filter)))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.Translations.Any(x => x.TranslatedDisplayName.Contains(input.DisplayNameFilter)));

            var pagedAndFilteredTrucksTypes = filteredTrucksTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var trucksTypes = from o in await pagedAndFilteredTrucksTypes.ToListAsync()
                              //join o1 in _transportTypeRepository.GetAll() on o.TransportTypeId equals o1.Id into j1
                              //from s1 in j1.DefaultIfEmpty()
                              select new GetTrucksTypeForViewDto()
                              {
                                  TrucksType=ObjectMapper.Map<TrucksTypeDto>(o),
                                  //TrucksType = new TrucksTypeDto
                                  //{
                                  //    DisplayName = o.DisplayName,
                                  //    Id = o.Id
                                  //},
                                  TransportTypeDisplayName = ObjectMapper.Map<TransportTypeDto>(o.TransportTypeFk).TranslatedDisplayName//s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
                              };

            var totalCount = await filteredTrucksTypes.CountAsync();

            return new PagedResultDto<GetTrucksTypeForViewDto>(
                totalCount,
                trucksTypes.ToList()
            );
        }

        public async Task<GetTrucksTypeForViewDto> GetTrucksTypeForView(long id)
        {
            var trucksType = await _trucksTypeRepository.GetAsync(id);

            var output = new GetTrucksTypeForViewDto { TrucksType = ObjectMapper.Map<TrucksTypeDto>(trucksType) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_TrucksTypes_Edit)]
        public async Task<GetTrucksTypeForEditOutput> GetTrucksTypeForEdit(EntityDto<long> input)
        {
            var trucksType = await _trucksTypeRepository.GetAllIncluding(x=>x.Translations)
                .FirstOrDefaultAsync(x=>x.Id == input.Id);

            var output = new GetTrucksTypeForEditOutput { TrucksType = ObjectMapper.Map<CreateOrEditTrucksTypeDto>(trucksType) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTrucksTypeDto input)
        {
            foreach (var transItem in input.Translations)
            {
                var isDuplicateUserName = await _trucksTypeRepository
                   .FirstOrDefaultAsync(x => x.Translations.Any(x=>x.TranslatedDisplayName==transItem.TranslatedDisplayName)  && x.Id != input.Id);
                if (isDuplicateUserName != null)
                {
                    throw new UserFriendlyException(string.Format(L("TrucksTypeDuplicateName"), transItem.TranslatedDisplayName));
                }
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

        [AbpAuthorize(AppPermissions.Pages_TrucksTypes_Create)]
        protected virtual async Task Create(CreateOrEditTrucksTypeDto input)
        {
            var trucksType = ObjectMapper.Map<TrucksType>(input);
            await _trucksTypeRepository.InsertAsync(trucksType);
        }

        [AbpAuthorize(AppPermissions.Pages_TrucksTypes_Edit)]
        protected virtual async Task Update(CreateOrEditTrucksTypeDto input)
        {
            var trucksType = await _trucksTypeRepository.GetAllIncluding(x=>x.Translations)
                .FirstOrDefaultAsync(x=>x.Id== input.Id.Value);
            trucksType.Translations.Clear();
            ObjectMapper.Map(input, trucksType);
        }

        [AbpAuthorize(AppPermissions.Pages_TrucksTypes_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _trucksTypeRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_TrucksTypes)]


        public async Task<IEnumerable<ISelectItemDto>> GetAllTransportTypeForTableDropdown()
        {
            List<TransportType> transportTypes = await _transportTypeRepository
                .GetAllIncluding(x =>x.Translations)
                .ToListAsync();

            List<TransportTypeSelectItemDto> transportTypeDtos = ObjectMapper.Map<List<TransportTypeSelectItemDto>>(transportTypes);

            return transportTypeDtos;
        }
    }
}