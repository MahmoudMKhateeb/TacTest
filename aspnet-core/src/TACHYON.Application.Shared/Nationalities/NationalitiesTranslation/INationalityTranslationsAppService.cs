using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Nationalities.NationalitiesTranslation.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;

namespace TACHYON.Nationalities.NationalitiesTranslation
{
    public interface INationalityTranslationsAppService : IApplicationService
    {
        Task<PagedResultDto<GetNationalityTranslationForViewDto>> GetAll(GetAllNationalityTranslationsInput input);

        Task<GetNationalityTranslationForViewDto> GetNationalityTranslationForView(int id);

        Task<GetNationalityTranslationForEditOutput> GetNationalityTranslationForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditNationalityTranslationDto input);

        Task Delete(EntityDto input);

        Task<List<NationalityTranslationNationalityLookupTableDto>> GetAllNationalityForTableDropdown();

    }
}