using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Countries.CountriesTranslations.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;

namespace TACHYON.Countries.CountriesTranslations
{
    public interface ICountriesTranslationsAppService : IApplicationService
    {
        Task<PagedResultDto<GetCountriesTranslationForViewDto>> GetAll(GetAllCountriesTranslationsInput input);

        Task<GetCountriesTranslationForViewDto> GetCountriesTranslationForView(int id);

        Task<GetCountriesTranslationForEditOutput> GetCountriesTranslationForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditCountriesTranslationDto input);

        Task Delete(EntityDto input);

        Task<List<CountriesTranslationCountyLookupTableDto>> GetAllCountyForTableDropdown();

    }
}