using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Receivers.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;

namespace TACHYON.Receivers
{
    public interface IReceiversAppService : IApplicationService
    {
        Task<PagedResultDto<GetReceiverForViewDto>> GetAll(GetAllReceiversInput input);

        Task<GetReceiverForViewDto> GetReceiverForView(int id);

        Task<GetReceiverForEditOutput> GetReceiverForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditReceiverDto input);

        Task Delete(EntityDto input);

        Task<FileDto> GetReceiversToExcel(GetAllReceiversForExcelInput input);

        Task<List<ReceiverFacilityLookupTableDto>> GetAllFacilityForTableDropdown(int? tenantId);
    }
}