using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Threading.Tasks;
using TACHYON.Countries.Dtos;
using TACHYON.Dto;


namespace TACHYON.Countries
{
    public interface ICountiesAppService : IApplicationService
    {
        Task<PagedResultDto<GetCountyForViewDto>> GetAll(GetAllCountiesInput input);

        Task<GetCountyForViewDto> GetCountyForView(int id);

        Task<GetCountyForEditOutput> GetCountyForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditCountyDto input);

        Task Delete(EntityDto input);

        Task<FileDto> GetCountiesToExcel(GetAllCountiesForExcelInput input);
    }
}