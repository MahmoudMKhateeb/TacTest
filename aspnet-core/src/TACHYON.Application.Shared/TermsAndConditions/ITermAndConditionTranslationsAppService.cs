using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.TermsAndConditions.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;

namespace TACHYON.TermsAndConditions
{
    public interface ITermAndConditionTranslationsAppService : IApplicationService
    {
        Task<PagedResultDto<GetTermAndConditionTranslationForViewDto>> GetAll(
            GetAllTermAndConditionTranslationsInput input);

        Task<GetTermAndConditionTranslationForViewDto> GetTermAndConditionTranslationForView(int id);

        Task<GetTermAndConditionTranslationForEditOutput> GetTermAndConditionTranslationForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditTermAndConditionTranslationDto input);

        Task Delete(EntityDto input);

        Task<List<TermAndConditionTranslationTermAndConditionLookupTableDto>> GetAllTermAndConditionForTableDropdown();
    }
}