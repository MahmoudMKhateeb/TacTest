using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Cities.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;


namespace TACHYON.Cities
{
    public interface ICitiesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetCityForViewDto>> GetAll(GetAllCitiesInput input);

        Task<GetCityForViewDto> GetCityForView(int id);

		Task<GetCityForEditOutput> GetCityForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditCityDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetCitiesToExcel(GetAllCitiesForExcelInput input);

		
		Task<List<CityCountyLookupTableDto>> GetAllCountyForTableDropdown();
		
    }
}