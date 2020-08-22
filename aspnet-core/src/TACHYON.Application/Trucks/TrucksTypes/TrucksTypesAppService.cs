

using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Dto;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Trucks.TrucksTypes
{
    [AbpAuthorize(AppPermissions.Pages_TrucksTypes)]
    public class TrucksTypesAppService : TACHYONAppServiceBase, ITrucksTypesAppService
    {
        private readonly IRepository<TrucksType, long> _trucksTypeRepository;


        public TrucksTypesAppService(IRepository<TrucksType, long> trucksTypeRepository)
        {
            _trucksTypeRepository = trucksTypeRepository;

        }

        public async Task<PagedResultDto<GetTrucksTypeForViewDto>> GetAll(GetAllTrucksTypesInput input)
        {

            var filteredTrucksTypes = _trucksTypeRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter);

            var pagedAndFilteredTrucksTypes = filteredTrucksTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var trucksTypes = from o in pagedAndFilteredTrucksTypes
                              select new GetTrucksTypeForViewDto()
                              {
                                  TrucksType = new TrucksTypeDto
                                  {
                                      DisplayName = o.DisplayName,
                                      Id = o.Id
                                  }
                              };

            var totalCount = await filteredTrucksTypes.CountAsync();

            return new PagedResultDto<GetTrucksTypeForViewDto>(
                totalCount,
                await trucksTypes.ToListAsync()
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
            var trucksType = await _trucksTypeRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTrucksTypeForEditOutput { TrucksType = ObjectMapper.Map<CreateOrEditTrucksTypeDto>(trucksType) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTrucksTypeDto input)
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

        [AbpAuthorize(AppPermissions.Pages_TrucksTypes_Create)]
        protected virtual async Task Create(CreateOrEditTrucksTypeDto input)
        {
            var trucksType = ObjectMapper.Map<TrucksType>(input);
            await _trucksTypeRepository.InsertAsync(trucksType);
        }

        [AbpAuthorize(AppPermissions.Pages_TrucksTypes_Edit)]
        protected virtual async Task Update(CreateOrEditTrucksTypeDto input)
        {
            var trucksType = await _trucksTypeRepository.FirstOrDefaultAsync(input.Id.Value);
            ObjectMapper.Map(input, trucksType);
        }

        [AbpAuthorize(AppPermissions.Pages_TrucksTypes_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _trucksTypeRepository.DeleteAsync(input.Id);
        }
    }
}