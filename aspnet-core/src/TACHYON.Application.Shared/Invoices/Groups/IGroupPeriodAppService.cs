using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Invoices.Groups.Dto;

namespace TACHYON.Invoices.Groups
{
    public interface IGroupPeriodAppService:IApplicationService
    {
        Task<PagedResultDto<GroupPeriodListDto>> GetAll(GroupPeriodFilterInput input);
        Task<GroupPeriodInfoDto> GetById(EntityDto input);

        Task Demand(GroupPeriodDemandCreateInput Input);
        Task UnDemand(long GroupId);
        Task Claim(long GroupId);
        Task UnClaim(long GroupId);

        Task Delete(EntityDto Input);

    }
}
