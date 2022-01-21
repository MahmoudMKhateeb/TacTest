using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.PickingTypes.Dtos;
using TACHYON.Dto;


namespace TACHYON.PickingTypes
{
    public interface IPickingTypesAppService : IApplicationService
    {
        Task<PagedResultDto<GetPickingTypeForViewDto>> GetAll(GetAllPickingTypesInput input);

        Task<GetPickingTypeForViewDto> GetPickingTypeForView(int id);

        Task<GetPickingTypeForEditOutput> GetPickingTypeForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditPickingTypeDto input);

        Task Delete(EntityDto input);
    }
}