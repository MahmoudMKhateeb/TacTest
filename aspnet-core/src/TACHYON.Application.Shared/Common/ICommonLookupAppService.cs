using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.Common.Dto;
using TACHYON.Editions.Dto;
using System.Collections.Generic;
using TACHYON.Dto;

namespace TACHYON.Common
{
    public interface ICommonLookupAppService : IApplicationService
    {
        Task<ListResultDto<SubscribableEditionComboboxItemDto>> GetEditionsForCombobox(bool onlyFreeItems = false);

        Task<PagedResultDto<NameValueDto>> FindUsers(FindUsersInput input);

        GetDefaultEditionNameOutput GetDefaultEditionName();

        IEnumerable<ISelectItemDto> GetAutoCompleteTenants(string name, string EdtionName);

        Task<List<SelectItemDto>> GetPeriods();

    }
}