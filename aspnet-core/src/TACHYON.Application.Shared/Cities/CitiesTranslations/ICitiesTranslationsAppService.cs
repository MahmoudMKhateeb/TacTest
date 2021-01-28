using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Cities.CitiesTranslations.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;

namespace TACHYON.Cities.CitiesTranslations
{
    public interface ICitiesTranslationsAppService : IApplicationService
    {
        Task<PagedResultDto<GetCitiesTranslationForViewDto>> GetAll(GetAllCitiesTranslationsInput input);

        Task<GetCitiesTranslationForViewDto> GetCitiesTranslationForView(int id);

        Task<GetCitiesTranslationForEditOutput> GetCitiesTranslationForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditCitiesTranslationDto input);

        Task Delete(EntityDto input);

        Task<List<CitiesTranslationCityLookupTableDto>> GetAllCityForTableDropdown();

    }
}