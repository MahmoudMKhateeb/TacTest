using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Cities.CitiesTranslations.Dtos;
using TACHYON.Dto;

namespace TACHYON.Cities.CitiesTranslations
{
    public interface ICitiesTranslationsAppService : IApplicationService
    {
        Task<LoadResult> GetAll(GetAllCitiesTranslationsInput input);
        Task CreateOrEdit(CreateOrEditCitiesTranslationDto input);
        Task<GetCitiesTranslationForViewDto> GetCitiesTranslationForView(int id);

        Task<GetCitiesTranslationForEditOutput> GetCitiesTranslationForEdit(EntityDto input);
        Task Delete(EntityDto input);

        Task<List<CitiesTranslationCityLookupTableDto>> GetAllCityForTableDropdown();
    }
}