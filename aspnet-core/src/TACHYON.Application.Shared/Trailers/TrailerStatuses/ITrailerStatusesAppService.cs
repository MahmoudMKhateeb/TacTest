using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Trailers.TrailerStatuses.Dtos;


namespace TACHYON.Trailers.TrailerStatuses
{
    public interface ITrailerStatusesAppService : IApplicationService
    {
        Task<PagedResultDto<GetTrailerStatusForViewDto>> GetAll(GetAllTrailerStatusesInput input);

        Task<GetTrailerStatusForViewDto> GetTrailerStatusForView(int id);

        Task<GetTrailerStatusForEditOutput> GetTrailerStatusForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditTrailerStatusDto input);

        Task Delete(EntityDto input);

        Task<FileDto> GetTrailerStatusesToExcel(GetAllTrailerStatusesForExcelInput input);


    }
}