using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.ShippingRequestVases.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;

namespace TACHYON.ShippingRequestVases
{
    public interface IShippingRequestVasesAppService : IApplicationService
    {
        Task<PagedResultDto<GetShippingRequestVasForViewDto>> GetAll(GetAllShippingRequestVasesInput input);

        Task<GetShippingRequestVasForViewDto> GetShippingRequestVasForView(long id);

        Task<GetShippingRequestVasForEditOutput> GetShippingRequestVasForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditShippingRequestVasDto input);

        Task Delete(EntityDto<long> input);

        Task<List<ShippingRequestVasVasLookupTableDto>> GetAllVasForTableDropdown();

    }
}