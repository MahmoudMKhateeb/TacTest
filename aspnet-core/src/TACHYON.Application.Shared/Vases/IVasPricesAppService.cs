using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Vases.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;


namespace TACHYON.Vases
{
    public interface IVasPricesAppService : IApplicationService
    {
        Task<PagedResultDto<GetVasPriceForViewDto>> GetAll(GetAllVasPricesInput input);

        Task<GetVasPriceForViewDto> GetVasPriceForView(int id);

        Task<GetVasPriceForEditOutput> GetVasPriceForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditVasPriceDto input);

        Task Delete(EntityDto input);

        Task<FileDto> GetVasPricesToExcel(GetAllVasPricesForExcelInput input);


        Task<List<VasPriceVasLookupTableDto>> GetAllVasForTableDropdown();
    }
}