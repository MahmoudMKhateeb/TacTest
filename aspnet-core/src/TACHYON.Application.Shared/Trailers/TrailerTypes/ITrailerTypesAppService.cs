using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Trailers.TrailerTypes.Dtos;
using TACHYON.Dto;


namespace TACHYON.Trailers.TrailerTypes
{
    public interface ITrailerTypesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetTrailerTypeForViewDto>> GetAll(GetAllTrailerTypesInput input);

        Task<GetTrailerTypeForViewDto> GetTrailerTypeForView(int id);

		Task<GetTrailerTypeForEditOutput> GetTrailerTypeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditTrailerTypeDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetTrailerTypesToExcel(GetAllTrailerTypesForExcelInput input);

		
    }
}