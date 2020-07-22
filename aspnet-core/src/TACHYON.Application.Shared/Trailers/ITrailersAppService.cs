using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Trailers.Dtos;


namespace TACHYON.Trailers
{
    public interface ITrailersAppService : IApplicationService
    {
        Task<PagedResultDto<GetTrailerForViewDto>> GetAll(GetAllTrailersInput input);

        Task<GetTrailerForViewDto> GetTrailerForView(long id);

        Task<GetTrailerForEditOutput> GetTrailerForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditTrailerDto input);

        Task Delete(EntityDto<long> input);

        Task<FileDto> GetTrailersToExcel(GetAllTrailersForExcelInput input);


        Task<List<TrailerTrailerStatusLookupTableDto>> GetAllTrailerStatusForTableDropdown();

        Task<List<TrailerTrailerTypeLookupTableDto>> GetAllTrailerTypeForTableDropdown();

        Task<List<TrailerPayloadMaxWeightLookupTableDto>> GetAllPayloadMaxWeightForTableDropdown();

        Task<List<TrailerTruckLookupTableDto>> GetAllTruckForTableDropdown();

    }
}