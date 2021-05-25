using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Goods.GoodsDetails.Dtos;


namespace TACHYON.Goods.GoodsDetails
{
    public interface IGoodsDetailsAppService : IApplicationService
    {
        Task<PagedResultDto<GetGoodsDetailForViewDto>> GetAll(GetAllGoodsDetailsInput input);

        Task<GetGoodsDetailForViewDto> GetGoodsDetailForView(long id);

        Task<GetGoodsDetailForEditOutput> GetGoodsDetailForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditGoodsDetailDto input);

        Task Delete(EntityDto<long> input);

        Task<FileDto> GetGoodsDetailsToExcel(GetAllGoodsDetailsForExcelInput input);

        Task<List<GetAllGoodsCategoriesForDropDownOutput>> GetAllGoodCategoryForTableDropdown(int? fatherId);

    }
}