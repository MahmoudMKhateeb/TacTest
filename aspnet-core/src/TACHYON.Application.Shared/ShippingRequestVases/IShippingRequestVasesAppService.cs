using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.ShippingRequestVases.Dtos;
using System.Collections.Generic;

namespace TACHYON.ShippingRequestVases
{
    public interface IShippingRequestVasesAppService : IApplicationService
    {
        Task<PagedResultDto<ShippingRequestVasDto>> GetAll(GetAllShippingRequestVasesInput input);

        Task<ShippingRequestVasDto> GetShippingRequestVasForView(long id); 

        Task<GetShippingRequestVasForEditOutput> GetShippingRequestVasForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditShippingRequestVasDto input);

        Task Delete(EntityDto<long> input);

        Task<List<ShippingRequestVasVasLookupTableDto>> GetAllVasForTableDropdown();

    }
}