using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Nationalities.Dtos;
using TACHYON.Dto;

namespace TACHYON.Nationalities
{
    public interface INationalitiesAppService : IApplicationService
    {
        Task<PagedResultDto<GetNationalityForViewDto>> GetAll(GetAllNationalitiesInput input);

        Task<GetNationalityForViewDto> GetNationalityForView(int id);

        Task<GetNationalityForEditOutput> GetNationalityForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditNationalityDto input);

        Task Delete(EntityDto input);

    }
}