using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Cities.Dtos;
using TACHYON.Common;
using TACHYON.Dto;


namespace TACHYON.Cities
{
    public interface ICitiesAppService : IApplicationService
    {
        Task<LoadResult> DxGetAll(LoadOptionsInput input);
        Task<GetCityForViewDto> GetCityForView(int id);

        Task<GetCityForEditOutput> GetCityForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditCityDto input);

        Task Delete(EntityDto input);

        Task<FileDto> GetCitiesToExcel(GetAllCitiesForExcelInput input);


        Task<List<CityCountyLookupTableDto>> GetAllCountyForTableDropdown();

    }
}