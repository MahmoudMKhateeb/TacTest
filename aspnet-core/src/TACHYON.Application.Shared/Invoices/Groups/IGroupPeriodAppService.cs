using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
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

        Task Claim(GroupPeriodClaimCreateInput Input);
       // Task UnDemand(long GroupId);
        Task Accepted(long GroupId);
        Task Rejected(SubmitInvoiceRejectedInput Input);

        Task Delete(EntityDto Input);
        Task<FileDto> GetFileDto(long GroupId);


    }
}
