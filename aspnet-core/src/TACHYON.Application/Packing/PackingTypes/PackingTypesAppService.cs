using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Packing.PackingTypes.Dtos;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Packing.PackingTypes
{
    [AbpAuthorize(AppPermissions.Pages_PackingTypes)]
    public class PackingTypesAppService : TACHYONAppServiceBase, IPackingTypesAppService
    {
        private readonly IRepository<PackingType> _packingTypeRepository;

        public PackingTypesAppService(IRepository<PackingType> packingTypeRepository)
        {
            _packingTypeRepository = packingTypeRepository;

        }

        public async Task<LoadResult> GetAll(GetAllPackingTypesInput input)
        {
             DisableTenancyFiltersIfHost();
            var filteredPackingTypes = _packingTypeRepository.GetAll()
                .ProjectTo<PackingTypeDto>(AutoMapperConfigurationProvider);


            return await LoadResultAsync(filteredPackingTypes, input.LoadOptions);



        }

        public async Task<GetPackingTypeForViewDto> GetPackingTypeForView(int id)
        {
            var packingType = await _packingTypeRepository.GetAsync(id);

            var output = new GetPackingTypeForViewDto { PackingType = ObjectMapper.Map<PackingTypeDto>(packingType) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_PackingTypes_Edit)]
        public async Task<GetPackingTypeForEditOutput> GetPackingTypeForEdit(EntityDto input)
        {
            var packingType = await _packingTypeRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetPackingTypeForEditOutput { PackingType = ObjectMapper.Map<CreateOrEditPackingTypeDto>(packingType) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditPackingTypeDto input)
        {
            await IsPackingTypeDuplicatedOrEmpty(input.DisplayName);

            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_PackingTypes_Create)]
        protected virtual async Task Create(CreateOrEditPackingTypeDto input)
        {
            var packingType = ObjectMapper.Map<PackingType>(input);

            await _packingTypeRepository.InsertAsync(packingType);
        }

        [AbpAuthorize(AppPermissions.Pages_PackingTypes_Edit)]
        protected virtual async Task Update(CreateOrEditPackingTypeDto input)
        {
            var packingType = await _packingTypeRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, packingType);
        }

        [AbpAuthorize(AppPermissions.Pages_PackingTypes_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _packingTypeRepository.DeleteAsync(input.Id);
        }


        private async Task IsPackingTypeDuplicatedOrEmpty(string displayName)
        {
            if (displayName.IsNullOrEmpty() || displayName.IsNullOrWhiteSpace())
                throw new UserFriendlyException(L("PackingTypeNameCanNotBeEmpty"));


            var isDuplicated = await _packingTypeRepository.GetAll()
                .AnyAsync(x => x.DisplayName.ToUpper().Equals(displayName.ToUpper()));

            if (isDuplicated)
                throw new UserFriendlyException(L("PackingTypeNameCanNotBeDuplicated"));
        }
    }
}