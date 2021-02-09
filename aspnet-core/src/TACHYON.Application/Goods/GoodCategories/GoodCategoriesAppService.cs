

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
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Goods.GoodCategories.Exporting;

namespace TACHYON.Goods.GoodCategories
{
    [AbpAuthorize(AppPermissions.Pages_GoodCategories)]
    public class GoodCategoriesAppService : TACHYONAppServiceBase, IGoodCategoriesAppService
    {
        private readonly IRepository<GoodCategory> _goodCategoryRepository;
        private readonly IGoodCategoriesExcelExporter _goodCategoriesExcelExporter;


        public GoodCategoriesAppService(IRepository<GoodCategory> goodCategoryRepository, IGoodCategoriesExcelExporter goodCategoriesExcelExporter)
        {
            _goodCategoryRepository = goodCategoryRepository;
            _goodCategoriesExcelExporter = goodCategoriesExcelExporter;

        }

        public async Task<PagedResultDto<GetGoodCategoryForViewDto>> GetAll(GetAllGoodCategoriesInput input)
        {

            var filteredGoodCategories = _goodCategoryRepository.GetAll()
                        .Include(e=>e.FatherFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter);

            var pagedAndFilteredGoodCategories = filteredGoodCategories
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var goodCategories = from o in pagedAndFilteredGoodCategories
                                 select new GetGoodCategoryForViewDto()
                                 {
                                     GoodCategory = new GoodCategoryDto
                                     {
                                         DisplayName = o.DisplayName,
                                         Id = o.Id,
                                         FatherId=o.FatherId
                                     },
                                     FatherCategoryName = o.FatherId != null ? o.FatherFk.DisplayName :"",
                                 };

            var totalCount = await filteredGoodCategories.CountAsync();

            return new PagedResultDto<GetGoodCategoryForViewDto>(
                totalCount,
                await goodCategories.ToListAsync()
            );
        }

        public async Task<GetGoodCategoryForViewDto> GetGoodCategoryForView(int id)
        {
            var goodCategory = await _goodCategoryRepository.GetAsync(id);

            var output = new GetGoodCategoryForViewDto {
                GoodCategory = ObjectMapper.Map<GoodCategoryDto>(goodCategory),
            FatherCategoryName=goodCategory.FatherFk?.DisplayName
            };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_GoodCategories_Edit)]
        public async Task<GetGoodCategoryForEditOutput> GetGoodCategoryForEdit(EntityDto input)
        {
            var goodCategory = await _goodCategoryRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetGoodCategoryForEditOutput { GoodCategory = ObjectMapper.Map<CreateOrEditGoodCategoryDto>(goodCategory) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditGoodCategoryDto input)
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

        [AbpAuthorize(AppPermissions.Pages_GoodCategories_Create)]
        protected virtual async Task Create(CreateOrEditGoodCategoryDto input)
        {
            var goodCategory = ObjectMapper.Map<GoodCategory>(input);



            await _goodCategoryRepository.InsertAsync(goodCategory);
        }

        [AbpAuthorize(AppPermissions.Pages_GoodCategories_Edit)]
        protected virtual async Task Update(CreateOrEditGoodCategoryDto input)
        {
            if (input.FatherId == input.Id)
            {
                throw new UserFriendlyException(L("Category cannot be father to itself"));
            }
            var goodCategory = await _goodCategoryRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, goodCategory);
        }

        [AbpAuthorize(AppPermissions.Pages_GoodCategories_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _goodCategoryRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetGoodCategoriesToExcel(GetAllGoodCategoriesForExcelInput input)
        {

            var filteredGoodCategories = _goodCategoryRepository.GetAll()
                        .Include(x=>x.FatherFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter);

            var query = (from o in filteredGoodCategories
                         select new GetGoodCategoryForViewDto()
                         {
                             GoodCategory = new GoodCategoryDto
                             {
                                 DisplayName = o.DisplayName,
                                 Id = o.Id,
                                 FatherId=o.FatherId
                             },
                             FatherCategoryName=o.FatherId!=null ?o.FatherFk.DisplayName :""
                         });


            var goodCategoryListDtos = await query.ToListAsync();

            return _goodCategoriesExcelExporter.ExportToFile(goodCategoryListDtos);
        }

        
        public async Task<List<GetAllGoodsCategoriesForDropDownOutput>> GetAllGoodsCategoriesForDropDown()
        {
            return await  _goodCategoryRepository.GetAll()
                .Select(x => new GetAllGoodsCategoriesForDropDownOutput
            {
                DisplayName = x.DisplayName,
                Id = x.Id
            }).ToListAsync();
        }
        
    }
}