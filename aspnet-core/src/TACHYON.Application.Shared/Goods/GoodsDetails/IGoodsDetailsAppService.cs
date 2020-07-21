using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Goods.GoodsDetails.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;


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

		
		Task<List<GoodsDetailGoodCategoryLookupTableDto>> GetAllGoodCategoryForTableDropdown();
		
    }
}