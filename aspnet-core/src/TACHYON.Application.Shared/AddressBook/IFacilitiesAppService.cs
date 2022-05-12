using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.AddressBook.Dtos;
using TACHYON.Dto;


namespace TACHYON.AddressBook
{
    public interface IFacilitiesAppService : IApplicationService
    {
        Task<PagedResultDto<GetFacilityForViewOutput>> GetAll(GetAllFacilitiesInput input);

        Task<GetFacilityForViewOutput> GetFacilityForView(long id);

        Task<GetFacilityForEditOutput> GetFacilityForEdit(EntityDto<long> input);

        Task<long> CreateOrEdit(CreateOrEditFacilityDto input);

        Task Delete(EntityDto<long> input);

        Task<FileDto> GetFacilitiesToExcel(GetAllFacilitiesForExcelInput input);


        Task<List<FacilityCityLookupTableDto>> GetAllCityForTableDropdown();
    }
}