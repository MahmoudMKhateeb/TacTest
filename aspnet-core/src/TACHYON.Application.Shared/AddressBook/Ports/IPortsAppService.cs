using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.AddressBook.Ports.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;


namespace TACHYON.AddressBook.Ports
{
    public interface IPortsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetPortForViewDto>> GetAll(GetAllPortsInput input);

        Task<GetPortForViewDto> GetPortForView(long id);

		Task<GetPortForEditOutput> GetPortForEdit(EntityDto<long> input);

		Task CreateOrEdit(CreateOrEditPortDto input);

		Task Delete(EntityDto<long> input);

		Task<FileDto> GetPortsToExcel(GetAllPortsForExcelInput input);

		
		Task<List<PortCityLookupTableDto>> GetAllCityForTableDropdown();
		
    }
}