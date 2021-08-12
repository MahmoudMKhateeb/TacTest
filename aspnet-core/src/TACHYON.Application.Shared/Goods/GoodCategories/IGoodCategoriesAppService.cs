using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Goods.GoodCategories.Dtos;


namespace TACHYON.Goods.GoodCategories
{
    public interface IGoodCategoriesAppService : IApplicationService
    {
        Task<PagedResultDto<GetGoodCategoryForViewDto>> GetAll(GetAllGoodCategoriesInput input);

        Task<GetGoodCategoryForViewDto> GetGoodCategoryForView(int id);

        Task<GetGoodCategoryForEditOutput> GetGoodCategoryForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditGoodCategoryDto input);

       // Task Delete(EntityDto input);

        Task<FileDto> GetGoodCategoriesToExcel(GetAllGoodCategoriesForExcelInput input);

        Task<List<GetAllGoodsCategoriesForDropDownOutput>> GetAllGoodsCategoriesForDropDown();
    }
}