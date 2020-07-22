using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Offers.Dtos;


namespace TACHYON.Offers
{
    public interface IOffersAppService : IApplicationService
    {
        Task<PagedResultDto<GetOfferForViewDto>> GetAll(GetAllOffersInput input);

        Task<GetOfferForViewDto> GetOfferForView(int id);

        Task<GetOfferForEditOutput> GetOfferForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditOfferDto input);

        Task Delete(EntityDto input);

        Task<FileDto> GetOffersToExcel(GetAllOffersForExcelInput input);


        Task<List<OfferTrucksTypeLookupTableDto>> GetAllTrucksTypeForTableDropdown();

        Task<List<OfferTrailerTypeLookupTableDto>> GetAllTrailerTypeForTableDropdown();

        Task<List<OfferGoodCategoryLookupTableDto>> GetAllGoodCategoryForTableDropdown();

        Task<List<OfferRouteLookupTableDto>> GetAllRouteForTableDropdown();

    }
}