using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Shipping.ShippingTypes.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Authorization;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using TACHYON.Common;

namespace TACHYON.Shipping.ShippingTypes
{
    [AbpAuthorize(AppPermissions.Pages_ShippingTypes)]
    public class ShippingTypesAppService : TACHYONAppServiceBase, IShippingTypesAppService
    {
        private readonly IRepository<ShippingType> _shippingTypeRepository;
        private readonly IRepository<ShippingTypeTranslation> _shippingTypeTranslationRepository;

        public ShippingTypesAppService(IRepository<ShippingType> shippingTypeRepository, IRepository<ShippingTypeTranslation> shippingTypeTranslationRepository)
        {
            _shippingTypeRepository = shippingTypeRepository;
            _shippingTypeTranslationRepository = shippingTypeTranslationRepository;
        }

        public async Task<LoadResult> GetAll(LoadOptionsInput input)
        {

            var query = _shippingTypeRepository.GetAll()
                .ProjectTo<ShippingTypeDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(query, input.LoadOptions);
        }

        public async Task CreateOrEdit(CreateOrEditShippingTypeDto input)
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

        [AbpAuthorize(AppPermissions.Pages_ShippingTypes_Create)]
        protected virtual async Task Create(CreateOrEditShippingTypeDto input)
        {
            var shippingType = ObjectMapper.Map<ShippingType>(input);

            await _shippingTypeRepository.InsertAsync(shippingType);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingTypes_Edit)]
        protected virtual async Task Update(CreateOrEditShippingTypeDto input)
        {
            var shippingType = await _shippingTypeRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, shippingType);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingTypes_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _shippingTypeRepository.DeleteAsync(input.Id);
        }

        public async Task<List<SelectItemDto>> GetAllShippingTypesForDropdown()
        {
            return await _shippingTypeRepository.GetAll()
                .Select(x => new SelectItemDto()
                {
                    Id = x.Id.ToString(),
                    DisplayName = x.DisplayName
                }).ToListAsync();
        }

        #region Translations


        public async Task<LoadResult> GetAllTranslations(GetAllTranslationsInput input)
        {
            var filteredTruckStatusTranslations = _shippingTypeTranslationRepository
                .GetAll().AsNoTracking()
                .Where(x => x.CoreId == input.CoreId)
                .ProjectTo<ShippingTypeTranslationDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(filteredTruckStatusTranslations, input.LoadOptions);
        }

        public async Task CreateOrEditTranslation(CreateOrEditShippingTypeTranslationDto input)
        {

            if (!input.Id.HasValue)
            {
                var d = await _shippingTypeTranslationRepository
                    .GetAll()
                    .Where(x => x.CoreId == input.CoreId)
                    .Where(x => x.DisplayName == input.DisplayName)
                    .Where(x => x.Language.Contains(input.Language))
                    .FirstOrDefaultAsync();
                if (d != null)
                {
                    throw new UserFriendlyException(L("TranslationDuplicated"));
                }

                var createdTranslation = ObjectMapper.Map<ShippingTypeTranslation>(input);
                await _shippingTypeTranslationRepository.InsertAsync(createdTranslation);
            }
            else
            {
                var updatedTranslation = await _shippingTypeTranslationRepository.SingleAsync(x => x.Id == input.Id);
                ObjectMapper.Map(input, updatedTranslation);
            }


        }

        public async Task DeleteTranslation(EntityDto input)
        {
            // Here I Used Single Async

            var deletedTranslation = await _shippingTypeTranslationRepository
                .SingleAsync(x => x.Id == input.Id);

            await _shippingTypeTranslationRepository.DeleteAsync(deletedTranslation);
        }

        #endregion

    }
}