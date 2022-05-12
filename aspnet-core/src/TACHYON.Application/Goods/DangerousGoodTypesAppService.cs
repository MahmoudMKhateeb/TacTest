using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Common;
using TACHYON.Dto;
using TACHYON.Goods.Dtos;

namespace TACHYON.Goods
{
    //[AbpAuthorize(AppPermissions.Pages_DangerousGoodTypes)]
    public class DangerousGoodTypesAppService : TACHYONAppServiceBase
    {
        private readonly IRepository<DangerousGoodType> _dangerousGoodTypeRepository;
        private readonly IRepository<DangerousGoodTypeTranslation> _dangerousGoodTypeTranslationRepository;

        // todo Add Mapping Configurations
        // todo Add Required Migrations
        public DangerousGoodTypesAppService(IRepository<DangerousGoodType> dangerousGoodTypeRepository,
            IRepository<DangerousGoodTypeTranslation> dangerousGoodTypeTranslationRepository)
        {
            _dangerousGoodTypeRepository = dangerousGoodTypeRepository;
            _dangerousGoodTypeTranslationRepository = dangerousGoodTypeTranslationRepository;
        }

        public async Task<LoadResult> GetAll(LoadOptionsInput input)
        {
            var query = _dangerousGoodTypeRepository.GetAll()
                .ProjectTo<DangerousGoodTypeDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(query, input.LoadOptions);
        }


        public async Task CreateOrEdit(CreateOrEditDangerousGoodTypeDto input)
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

        [AbpAuthorize(AppPermissions.Pages_DangerousGoodTypes_Create)]
        protected virtual async Task Create(CreateOrEditDangerousGoodTypeDto input)
        {
            var dangerousGoodType = ObjectMapper.Map<DangerousGoodType>(input);


            await _dangerousGoodTypeRepository.InsertAsync(dangerousGoodType);
        }

        [AbpAuthorize(AppPermissions.Pages_DangerousGoodTypes_Edit)]
        protected virtual async Task Update(CreateOrEditDangerousGoodTypeDto input)
        {
            var dangerousGoodType = await _dangerousGoodTypeRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, dangerousGoodType);
        }

        [AbpAuthorize(AppPermissions.Pages_DangerousGoodTypes_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _dangerousGoodTypeRepository.DeleteAsync(input.Id);
        }

        public async Task<List<SelectItemDto>> GetAllForDropdownList()
        {
            return await _dangerousGoodTypeRepository.GetAll().Select(x => new SelectItemDto
            {
                DisplayName = x.Name, Id = x.Id.ToString()
            }).ToListAsync();
        }

        public async Task<LoadResult> GetAllTranslation(GetAllDangerousGoodTypeTranslationsInput input)
        {
            var query = _dangerousGoodTypeTranslationRepository.GetAll()
                .Where(x => x.CoreId == input.CoreId)
                .ProjectTo<DangerousGoodTypeTranslationDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(query, input.LoadOptions);
        }

        public async Task CreateOrEditTranslation(CreateOrEditDangerousGoodTypeTranslationDto input)
        {
            var isCoreEntityExist = await _dangerousGoodTypeRepository.GetAll()
                .AnyAsync(x => x.Id == input.CoreId);
            if (!isCoreEntityExist)
                throw new UserFriendlyException(L("NoSuchDangerousGoodTypeFound"));

            if (!input.Id.HasValue)
                await CreateTranslation(input);
            else
                await UpdateTranslation(input);
        }

        protected virtual async Task CreateTranslation(CreateOrEditDangerousGoodTypeTranslationDto input)
        {
            var dangerousGoodTypeTranslation = ObjectMapper.Map<DangerousGoodTypeTranslation>(input);

            await _dangerousGoodTypeTranslationRepository.InsertAsync(dangerousGoodTypeTranslation);
        }

        protected virtual async Task UpdateTranslation(CreateOrEditDangerousGoodTypeTranslationDto input)
        {
            var updatedGoodTypeTranslation =
                await _dangerousGoodTypeTranslationRepository.FirstOrDefaultAsync(input.Id.Value);

            ObjectMapper.Map(input, updatedGoodTypeTranslation);
        }

        public async Task DeleteTranslation(EntityDto input)
        {
            var deletedTranslation = await _dangerousGoodTypeTranslationRepository
                .SingleAsync(x => x.Id == input.Id);

            await _dangerousGoodTypeTranslationRepository.DeleteAsync(deletedTranslation);
        }
    }
}