using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Shipping.ShippingRequests.Dtos;


namespace TACHYON.Shipping.ShippingRequests
{
    public interface IShippingRequestsAppService : IApplicationService
    {
        Task<PagedResultDto<GetShippingRequestForViewDto>> GetAll(GetAllShippingRequestsInput input);

        Task<GetShippingRequestForViewDto> GetShippingRequestForView(long id);

        Task<GetShippingRequestForEditOutput> GetShippingRequestForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditShippingRequestDto input);

        Task Delete(EntityDto<long> input);

        Task<FileDto> GetShippingRequestsToExcel(GetAllShippingRequestsForExcelInput input);


        Task<List<ShippingRequestTrucksTypeLookupTableDto>> GetAllTrucksTypeForTableDropdown();

        Task<List<ShippingRequestTrailerTypeLookupTableDto>> GetAllTrailerTypeForTableDropdown();

        Task<List<ShippingRequestGoodsDetailLookupTableDto>> GetAllGoodsDetailForTableDropdown();

        Task<List<ShippingRequestRouteLookupTableDto>> GetAllRouteForTableDropdown();

    }
}