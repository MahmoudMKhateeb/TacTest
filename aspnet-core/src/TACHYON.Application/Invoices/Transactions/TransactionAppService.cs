using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Invoices.Transactions.Dto;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Abp.Linq.Extensions;
using System.Linq;
using Abp.Authorization;
using TACHYON.Authorization;
using TACHYON.Common;
using TACHYON.Dto;
using TACHYON.Invoices.Balances.Exporting;

namespace TACHYON.Invoices.Transactions
{
    public class TransactionAppService : TACHYONAppServiceBase, ITransactionAppService
    {
        private readonly IRepository<Transaction, long> _transactionRepository;
        private readonly CommonManager _commonManager;
        private readonly ITransactionExcelExporter _transactionExcelExporter;

        public TransactionAppService(
            IRepository<Transaction, long> transactionRepository,
            CommonManager commonManager,
            ITransactionExcelExporter transactionExcelExporter)
        {
            _transactionRepository = transactionRepository;
            _commonManager = commonManager;
            _transactionExcelExporter = transactionExcelExporter;
        }

        [AbpAuthorize(AppPermissions.Pages_Invoices_Transaction)]
        public async Task<PagedResultDto<TransactionListDto>> GetAll(TransactionFilterInput input)
        {
           var query = GetFilterTransactions(input);
            var pages = query.PageBy(input);

            var totalCount = await query.CountAsync();

            return new PagedResultDto<TransactionListDto>(
                totalCount,
                ObjectMapper.Map<List<TransactionListDto>>(pages)
            );
        }


        [AbpAuthorize(AppPermissions.Pages_Invoices_Transaction)]
        public Task<FileDto> Exports(TransactionFilterInput input)
        {
            var query = GetFilterTransactions(input);
            var data = ObjectMapper.Map<List<TransactionListDto>>(query);
            return Task.FromResult(_transactionExcelExporter.ExportToFile(data));

        }
        private IOrderedQueryable<Transaction> GetFilterTransactions(TransactionFilterInput input)
        {
            DisableTenancyFilters();
            var query = _transactionRepository
                .GetAll()
                .Include(t=>t.Tenant)
                 .ThenInclude(e=>e.Edition)
                .WhereIf(AbpSession.TenantId.HasValue, i => i.TenantId == AbpSession.TenantId.Value)
                .WhereIf(input.channelType.HasValue, e => e.ChannelId == input.channelType)
                .WhereIf(input.TenantId.HasValue, i => i.TenantId == input.TenantId)
                .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue, i => i.CreationTime >= input.FromDate.Value && i.CreationTime <= input.ToDate.Value)
                .WhereIf(input.minLongitude.HasValue && input.maxLongitude.HasValue, i => i.Amount >= input.minLongitude.Value && i.Amount <= input.maxLongitude.Value)
                .WhereIf(input.EditionId.HasValue,e=>e.Tenant.EditionId== input.EditionId.Value)
                .AsNoTracking()
                .OrderBy(!string.IsNullOrEmpty(input.Sorting)? input.Sorting : "CreationTime desc");

            return query;
        }
    }
}
