using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Shipping.ShippingTypes.Dtos;
using TACHYON.Dto;

namespace TACHYON.Shipping.ShippingTypes
{
    public interface IShippingTypesAppService : IApplicationService
    {
        Task<PagedResultDto<GetShippingTypeForViewDto>> GetAll(GetAllShippingTypesInput input);

        Task<GetShippingTypeForViewDto> GetShippingTypeForView(int id);

        Task<GetShippingTypeForEditOutput> GetShippingTypeForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditShippingTypeDto input);

        Task Delete(EntityDto input);

    }
}