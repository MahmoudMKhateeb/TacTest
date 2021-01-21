using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Nationalities.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Nationalities
{
    //[AbpAuthorize(AppPermissions.Pages_Nationalities)]
    public class NationalitiesAppService : TACHYONAppServiceBase, INationalitiesAppService
    {
        private readonly IRepository<Nationality> _nationalityRepository;

        public NationalitiesAppService(IRepository<Nationality> nationalityRepository)
        {
            _nationalityRepository = nationalityRepository;

        }

        public async Task<PagedResultDto<GetNationalityForViewDto>> GetAll(GetAllNationalitiesInput input)
        {

            var filteredNationalities = _nationalityRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter);

            var pagedAndFilteredNationalities = filteredNationalities
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var nationalities = from o in pagedAndFilteredNationalities
                                select new GetNationalityForViewDto()
                                {
                                    Nationality = new NationalityDto
                                    {
                                        Name = o.Name,
                                        Id = o.Id
                                    }
                                };

            var totalCount = await filteredNationalities.CountAsync();

            return new PagedResultDto<GetNationalityForViewDto>(
                totalCount,
                await nationalities.ToListAsync()
            );
        }

        public async Task<GetNationalityForViewDto> GetNationalityForView(int id)
        {
            var nationality = await _nationalityRepository.GetAsync(id);

            var output = new GetNationalityForViewDto { Nationality = ObjectMapper.Map<NationalityDto>(nationality) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Nationalities_Edit)]
        public async Task<GetNationalityForEditOutput> GetNationalityForEdit(EntityDto input)
        {
            var nationality = await _nationalityRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetNationalityForEditOutput { Nationality = ObjectMapper.Map<CreateOrEditNationalityDto>(nationality) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditNationalityDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Nationalities_Create)]
        protected virtual async Task Create(CreateOrEditNationalityDto input)
        {
            var nationality = ObjectMapper.Map<Nationality>(input);

            await _nationalityRepository.InsertAsync(nationality);
        }

        [AbpAuthorize(AppPermissions.Pages_Nationalities_Edit)]
        protected virtual async Task Update(CreateOrEditNationalityDto input)
        {
            var nationality = await _nationalityRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, nationality);
        }

        [AbpAuthorize(AppPermissions.Pages_Nationalities_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _nationalityRepository.DeleteAsync(input.Id);
        }

        public async Task<IEnumerable<ISelectItemDto>> GetAllNationalityForDropdown()
        {
            return await _nationalityRepository
                .GetAll()
                .Select(x => new SelectItemDto
                {
                    Id = x.Id.ToString(),
                    DisplayName = x.Name
                })
                .ToListAsync();

        }

    }
}