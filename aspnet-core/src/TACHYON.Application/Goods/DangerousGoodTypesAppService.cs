

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Goods.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using TACHYON.Common;

namespace TACHYON.Goods
{
    [AbpAuthorize(AppPermissions.Pages_DangerousGoodTypes)]
    public class DangerousGoodTypesAppService : TACHYONAppServiceBase
    {
        private readonly IRepository<DangerousGoodType> _dangerousGoodTypeRepository;


        public DangerousGoodTypesAppService(IRepository<DangerousGoodType> dangerousGoodTypeRepository)
        {
            _dangerousGoodTypeRepository = dangerousGoodTypeRepository;

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

        [AbpAllowAnonymous]
        public async Task<List<SelectItemDto>> GetAllForDropdownList()
        {
            return await _dangerousGoodTypeRepository.GetAll().Select(x => new SelectItemDto
            {
                DisplayName = x.Name,
                Id = x.Id.ToString()
            }).ToListAsync();
        }
    }
}