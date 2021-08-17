using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DevExtreme.AspNet.Data.ResponseModel;
using TACHYON.Trucks.TruckStatusesTranslations.Dtos;
using System.Collections.Generic;

namespace TACHYON.Trucks.TruckStatusesTranslations
{
    public interface ITruckStatusesTranslationsAppService : IApplicationService
    {
        Task<LoadResult> GetAll(GetAllTruckStatusesTranslationsInput input);

        Task<GetTruckStatusesTranslationForViewDto> GetTruckStatusesTranslationForView(int id);

        Task<GetTruckStatusesTranslationForEditOutput> GetTruckStatusesTranslationForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditTruckStatusesTranslationDto input);

        Task Delete(EntityDto input);

        Task<List<TruckStatusesTranslationTruckStatusLookupTableDto>> GetAllTruckStatusForTableDropdown();

    }
}