using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DevExtreme.AspNet.Data.ResponseModel;
using System.Threading.Tasks;
using TACHYON.Common;
using TACHYON.Common.Dto;
using TACHYON.Trucks.TrucksTypes.Dtos;
using TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations.Dtos;


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

        Task<LoadResult> GetAllTranslations(GetAllTranslationInput<long> input);

        Task CreateOrEditTranslation(CreateOrEditTrucksTypesTranslationDto input);

        Task DeleteTranslation(EntityDto input);
    }
}