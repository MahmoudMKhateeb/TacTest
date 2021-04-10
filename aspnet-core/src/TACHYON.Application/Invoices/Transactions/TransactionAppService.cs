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
            var query =  _commonManager.ExecuteMethodIfHostOrTenantUsers(() =>
             {
                 return GetFilterTransactions(input);
             });
            var pages = query.PageBy(input);

            var totalCount = await pages.CountAsync();

            return new PagedResultDto<TransactionListDto>(
                totalCount,
                ObjectMapper.Map<List<TransactionListDto>>(pages)
            );
        }


        [AbpAuthorize(AppPermissions.Pages_Invoices_Transaction)]
        public Task<FileDto> Exports(TransactionFilterInput input)
        {
            var query = _commonManager.ExecuteMethodIfHostOrTenantUsers(() =>
            {
                return GetFilterTransactions(input);
            });
            var data = ObjectMapper.Map<List<TransactionListDto>>(query);
            return Task.FromResult(_transactionExcelExporter.ExportToFile(data));

        }
        private IOrderedQueryable<Transaction> GetFilterTransactions(TransactionFilterInput input)
        {
            var query = _transactionRepository
                .GetAll()
                .Include(t=>t.Tenant)
                .WhereIf(input.channelType.HasValue, e => e.ChannelId == input.channelType)
                .WhereIf(input.TenantId.HasValue, i => i.TenantId == input.TenantId)
                .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue, i => i.CreationTime >= input.FromDate && i.CreationTime < input.ToDate)
                .AsNoTracking()
                .OrderBy(!string.IsNullOrEmpty(input.Sorting)? input.Sorting : "CreationTime desc");

            return query;
        }
    }
}
