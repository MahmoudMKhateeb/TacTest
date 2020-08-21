using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.AddressBook.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;
using System.Collections.Generic;


namespace TACHYON.AddressBook
{
    public interface IFacilitiesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetFacilityForViewDto>> GetAll(GetAllFacilitiesInput input);

        Task<GetFacilityForViewDto> GetFacilityForView(long id);

		Task<GetFacilityForEditOutput> GetFacilityForEdit(EntityDto<long> input);

		Task CreateOrEdit(CreateOrEditFacilityDto input);

		Task Delete(EntityDto<long> input);

		Task<FileDto> GetFacilitiesToExcel(GetAllFacilitiesForExcelInput input);

		
		Task<List<FacilityCountyLookupTableDto>> GetAllCountyForTableDropdown();
		
		Task<List<FacilityCityLookupTableDto>> GetAllCityForTableDropdown();
		
    }
}