using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Dto;
using TACHYON.Invoices.Balances.Dto;
using TACHYON.Invoices.Balances.Exporting;
using TACHYON.Invoices.Transactions;

namespace TACHYON.Invoices.Balances
{
    public class BalanceRechargeAppService : TACHYONAppServiceBase, IBalanceRechargeAppService
    {
        private readonly IRepository<BalanceRecharge> _Repository;
        private readonly BalanceManager _balanceManager;
        private readonly IBalanceRechargeExcelExporter  _BalanceRechargeExcelExporter;
        private readonly TransactionManager _transactionManager;

        public BalanceRechargeAppService(
            IRepository<BalanceRecharge> Repository,
            BalanceManager balanceManager,
            IBalanceRechargeExcelExporter BalanceRechargeExcelExporter,
            TransactionManager transactionManager)
        {
            _Repository = Repository;
            _balanceManager = balanceManager;
            _BalanceRechargeExcelExporter = BalanceRechargeExcelExporter;
             _transactionManager= transactionManager;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_Balances)]
        public async Task<PagedResultDto<BalanceRechargeListDto>> GetAll(GetAllBalanceRechargeInput input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                var query = _Repository
                    .GetAll()
                    .Include(i => i.Tenant)
                    .WhereIf(input.TenantId.HasValue, i => i.TenantId == input.TenantId)
                    .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue, i => i.CreationTime >= input.FromDate && i.CreationTime < input.ToDate);

                var paged = query
                  .OrderBy(input.Sorting ?? "id desc")
                  .PageBy(input);

                var totalCount = await paged.CountAsync();

                return new PagedResultDto<BalanceRechargeListDto>(
                    totalCount,
                    ObjectMapper.Map<List<BalanceRechargeListDto>>(paged)
                );
            }
        }
        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_Balances_Create)]
        public async Task Create(CreateBalanceRechargeInput input)
        {
            var Recharge = ObjectMapper.Map<BalanceRecharge>(input);
           var id= await _Repository.InsertAndGetIdAsync(Recharge);
            await _balanceManager.AddBalanceToShipper(Recharge.TenantId, Recharge.Amount);
            await _transactionManager.Create(new Transaction
            {
                Amount = Recharge.Amount,
                ChannelId = (byte)ChannelType.BalanceRecharge,
                TenantId = Recharge.TenantId,
                SourceId = id,
            });
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_Balances_Delete)]
        public async Task Delete(EntityDto input)
        {
            var Recharge = await _Repository.SingleAsync(b => b.Id == input.Id);
            await _Repository.DeleteAsync(input.Id);
            await _balanceManager.AddBalanceToShipper(Recharge.TenantId, -Recharge.Amount);
            await _transactionManager.Delete(Recharge.Id, ChannelType.BalanceRecharge);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_Balances)]
        public Task<FileDto> Exports(GetAllBalanceRechargeInput input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                var query =  _Repository
                    .GetAll()
                    .Include(i => i.Tenant)
                    .WhereIf(input.TenantId.HasValue, i => i.TenantId == input.TenantId)
                    .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue, i => i.CreationTime >= input.FromDate && i.CreationTime < input.ToDate)
                    .OrderBy(!string.IsNullOrEmpty(input.Sorting) ? input.Sorting : "id desc");
                var data = ObjectMapper.Map<List<BalanceRechargeListDto>>(query);
                return Task.FromResult(_BalanceRechargeExcelExporter.ExportToFile(data));
            }

        }

    }
}
