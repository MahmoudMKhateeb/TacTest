﻿using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Invoices.Balances.Dto;

namespace TACHYON.Invoices.Balances
{
    public interface IBalanceRechargeAppService : IApplicationService
    {
        Task<PagedResultDto<BalanceRechargeListDto>> GetAll(GetAllBalanceRechargeInput input);

        Task Create(CreateBalanceRechargeInput input);
        Task Delete(EntityDto input);

        Task<FileDto> Exports(GetAllBalanceRechargeInput input);


    }
}