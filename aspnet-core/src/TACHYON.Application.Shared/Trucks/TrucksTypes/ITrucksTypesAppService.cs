using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DevExtreme.AspNet.Data.ResponseModel;
using System.Threading.Tasks;
using TACHYON.Common;
using TACHYON.Trucks.TrucksTypes.Dtos;


namespace TACHYON.Trucks.TrucksTypes
{
    public interface ITrucksTypesAppService : IApplicationService
    {
        Task<PagedResultDto<GetTrucksTypeForViewDto>> GetAll(GetAllTrucksTypesInput input);
        Task<LoadResult> DxGetAll(LoadOptionsInput input);
        
        Task<GetTrucksTypeForViewDto> GetTrucksTypeForView(long id);

        Task<GetTrucksTypeForEditOutput> GetTrucksTypeForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditTrucksTypeDto input);

        //Task Delete(EntityDto<long> input);


    }
}