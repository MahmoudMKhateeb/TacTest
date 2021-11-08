using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Common.Dto;
using TACHYON.Dto;
using TACHYON.Editions.Dto;
using TACHYON.Shipping.Accidents.Dto;

namespace TACHYON.Common
{
    public interface ICommonLookupAppService : IApplicationService
    {
        Task<ListResultDto<SubscribableEditionComboboxItemDto>> GetEditionsForCombobox(bool onlyFreeItems = false);

        Task<PagedResultDto<NameValueDto>> FindUsers(FindUsersInput input);

        GetDefaultEditionNameOutput GetDefaultEditionName();

        IEnumerable<ISelectItemDto> GetAutoCompleteTenants(string name, string EdtionName);

        Task<List<SelectItemDto>> GetPeriods();

        Task<List<ShippingRequestAccidentReasonLookupDto>> GetAccidentReason();

    }
}