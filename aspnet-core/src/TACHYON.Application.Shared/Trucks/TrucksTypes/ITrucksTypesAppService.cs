using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Trucks.TrucksTypes.Dtos;
using TACHYON.Dto;


namespace TACHYON.Trucks.TrucksTypes
{
    public interface ITrucksTypesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetTrucksTypeForViewDto>> GetAll(GetAllTrucksTypesInput input);

        Task<GetTrucksTypeForViewDto> GetTrucksTypeForView(Guid id);

		Task<GetTrucksTypeForEditOutput> GetTrucksTypeForEdit(EntityDto<Guid> input);

		Task CreateOrEdit(CreateOrEditTrucksTypeDto input);

		Task Delete(EntityDto<Guid> input);

		
    }
}