using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Trucks.TruckCategories.TruckCapacities.TruckCapacitiesTranslations.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;

namespace TACHYON.Trucks.TruckCategories.TruckCapacities.TruckCapacitiesTranslations
{
    public interface ITruckCapacitiesTranslationsAppService : IApplicationService
    {
        Task<PagedResultDto<GetTruckCapacitiesTranslationForViewDto>> GetAll(GetAllTruckCapacitiesTranslationsInput input);

        Task<GetTruckCapacitiesTranslationForViewDto> GetTruckCapacitiesTranslationForView(int id);

        Task<GetTruckCapacitiesTranslationForEditOutput> GetTruckCapacitiesTranslationForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditTruckCapacitiesTranslationDto input);

        Task Delete(EntityDto input);

        Task<List<TruckCapacitiesTranslationCapacityLookupTableDto>> GetAllCapacityForTableDropdown();

    }
}