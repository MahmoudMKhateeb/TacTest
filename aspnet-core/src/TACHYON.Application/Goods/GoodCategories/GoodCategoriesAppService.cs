using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Localization;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
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

        public GoodCategoriesAppService(IRepository<GoodCategory> goodCategoryRepository,
            IGoodCategoriesExcelExporter goodCategoriesExcelExporter)
        {
            _goodCategoryRepository = goodCategoryRepository;
            _goodCategoriesExcelExporter = goodCategoriesExcelExporter;
        }

        public async Task<PagedResultDto<GetGoodCategoryForViewDto>> GetAll(GetAllGoodCategoriesInput input)
        {
            var filteredGoodCategories = _goodCategoryRepository.GetAll()
                .Include(e => e.FatherFk)
                .ThenInclude(e => e.Translations)
                .Include(x => x.Translations)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    e => false || e.Translations.Any(x => x.DisplayName.Contains(input.Filter)))
                .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),
                    e => e.Translations.Any(x => x.DisplayName.Contains(input.DisplayNameFilter)));

            var pagedAndFilteredGoodCategories = filteredGoodCategories
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);


            var goodCategories = (await pagedAndFilteredGoodCategories.ToListAsync()).Select(o =>
                new GetGoodCategoryForViewDto()
                {
                    GoodCategory = ObjectMapper.Map<GoodCategoryDto>(o),
                    FatherCategoryName =
                        ObjectMapper.Map<GoodCategoryDto>(o.FatherFk)
                            ?.DisplayName //o.FatherId != null ? o.FatherFk.Translations.Where(x=>x.Language == culture).FirstOrDefault()?.DisplayName :"",
                });

            var totalCount = await filteredGoodCategories.CountAsync();

            return new PagedResultDto<GetGoodCategoryForViewDto>(
                totalCount,
                goodCategories.ToList()
            );
        }

        public async Task<GetGoodCategoryForViewDto> GetGoodCategoryForView(int id)
        {
            var goodCategory = await _goodCategoryRepository.GetAll().Include(x => x.Translations)
                .Include(x => x.FatherFk)
                .ThenInclude(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == id);

            var output = new GetGoodCategoryForViewDto
            {
                GoodCategory = ObjectMapper.Map<GoodCategoryDto>(goodCategory),
                FatherCategoryName = ObjectMapper.Map<GoodCategoryDto>(goodCategory.FatherFk).DisplayName
            };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_GoodCategories_Edit)]
        public async Task<GetGoodCategoryForEditOutput> GetGoodCategoryForEdit(EntityDto input)
        {
            var goodCategory = await _goodCategoryRepository.GetAllIncluding(x => x.Translations)
                .FirstOrDefaultAsync(e => e.Id == input.Id);

            var output = new GetGoodCategoryForEditOutput
            {
                GoodCategory = ObjectMapper.Map<CreateOrEditGoodCategoryDto>(goodCategory)
            };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditGoodCategoryDto input)
        {
            await ValidateDuplicatedDisplayName(input);
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        private async Task ValidateDuplicatedDisplayName(CreateOrEditGoodCategoryDto input)
        {
            foreach (var transItem in input.Translations)
            {
                if (string.IsNullOrWhiteSpace(transItem.DisplayName))
                {
                    throw new UserFriendlyException(L("DisplayNameCannotBeEmpty"));
                }

                var isDuplicateUserName = await _goodCategoryRepository
                    .FirstOrDefaultAsync(x => x.Translations.Any(i => i.DisplayName == transItem.DisplayName) &&
                                              x.FatherId == input.FatherId &&
                                              x.Id != input.Id);
                if (isDuplicateUserName != null)
                {
                    throw new UserFriendlyException(string.Format(L("GoodsCategoryDuplicateName"),
                        transItem.DisplayName));
                }
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

            var goodCategory = await _goodCategoryRepository.GetAllIncluding(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == (int)input.Id);
            goodCategory.Translations.Clear();
            ObjectMapper.Map(input, goodCategory);
        }

        //[AbpAuthorize(AppPermissions.Pages_GoodCategories_Delete)]
        //public async Task Delete(EntityDto input)
        //{
        //    await _goodCategoryRepository.DeleteAsync(input.Id);
        //}

        public async Task<FileDto> GetGoodCategoriesToExcel(GetAllGoodCategoriesForExcelInput input)
        {
            var filteredGoodCategories = _goodCategoryRepository.GetAll()
                .Include(x => x.Translations)
                .Include(x => x.FatherFk)
                .ThenInclude(x => x.Translations)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    e => false || e.Translations.Any(x => x.DisplayName.Contains(input.Filter)))
                .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),
                    e => e.Translations.Any(x => x.DisplayName == input.DisplayNameFilter));

            var query = (from o in await filteredGoodCategories.ToListAsync()
                select new GetGoodCategoryForViewDto()
                {
                    GoodCategory = ObjectMapper.Map<GoodCategoryDto>(o),
                    FatherCategoryName = ObjectMapper.Map<GoodCategoryDto>(o.FatherFk).DisplayName
                });


            var goodCategoryListDtos = query.ToList();

            return _goodCategoriesExcelExporter.ExportToFile(goodCategoryListDtos);
        }


        public async Task<List<GetAllGoodsCategoriesForDropDownOutput>> GetAllGoodsCategoriesForDropDown()
        {
            //return await  _goodCategoryRepository.GetAll()
            //    .Select(x => new GetAllGoodsCategoriesForDropDownOutput
            //{
            //    DisplayName = x.DisplayName,
            //    Id = x.Id
            //}).ToListAsync();
            var list = await _goodCategoryRepository.GetAll()
                .Where(x => x.IsActive)
                .Include(x => x.Translations).ToListAsync();

            return ObjectMapper.Map<List<GetAllGoodsCategoriesForDropDownOutput>>(list);
        }

        // -----------------------

        public async Task<LoadResult> GetAllDx(DataSourceLoadOptionsBase loadOptions)
        {
            var query = _goodCategoryRepository.GetAll()
                .ProjectTo<GoodCategoryDto>(AutoMapperConfigurationProvider);
            var result = await DataSourceLoader.LoadAsync(query, loadOptions);
            return result;
        }
    }
}