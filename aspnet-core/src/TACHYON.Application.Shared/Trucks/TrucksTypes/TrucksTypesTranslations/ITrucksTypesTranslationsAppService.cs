using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;

namespace TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations
{
    public interface ITrucksTypesTranslationsAppService : IApplicationService
    {
        Task<PagedResultDto<GetTrucksTypesTranslationForViewDto>> GetAll(GetAllTrucksTypesTranslationsInput input);

        Task<GetTrucksTypesTranslationForViewDto> GetTrucksTypesTranslationForView(int id);

        Task<GetTrucksTypesTranslationForEditOutput> GetTrucksTypesTranslationForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditTrucksTypesTranslationDto input);

        Task Delete(EntityDto input);

        Task<List<TrucksTypesTranslationTrucksTypeLookupTableDto>> GetAllTrucksTypeForTableDropdown();

    }
}