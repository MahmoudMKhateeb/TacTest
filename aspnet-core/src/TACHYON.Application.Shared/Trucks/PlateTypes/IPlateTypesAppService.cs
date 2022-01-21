using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Trucks.PlateTypes.Dtos;
using TACHYON.Dto;

namespace TACHYON.Trucks.PlateTypes
{
    public interface IPlateTypesAppService : IApplicationService
    {
        Task<PagedResultDto<PlateTypeDto>> GetAll(GetAllPlateTypesInput input);

        Task<GetPlateTypeForViewDto> GetPlateTypeForView(int id);

        Task<GetPlateTypeForEditOutput> GetPlateTypeForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditPlateTypeDto input);

        Task Delete(EntityDto input);
    }
}