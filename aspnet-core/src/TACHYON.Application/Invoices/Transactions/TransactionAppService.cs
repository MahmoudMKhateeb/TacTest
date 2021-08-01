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
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
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
        public async Task<LoadResult> GetAll(TransactionFilterInput input)
        {
            DisableTenancyFiltersIfHost();
            var query = _transactionRepository
                 .GetAll()
                 .AsNoTracking()
                 .ProjectTo<TransactionListDto>(AutoMapperConfigurationProvider);
            return await LoadResultAsync(query, input.LoadOptions);
        }


        [AbpAuthorize(AppPermissions.Pages_Invoices_Transaction)]
        public async Task<FileDto> Exports(TransactionFilterInput input)
        {
            DisableTenancyFiltersIfHost();
            var query = _transactionRepository
                 .GetAll()
                 .AsNoTracking()
                 .ProjectTo<TransactionListDto>(AutoMapperConfigurationProvider);

            var data = (await LoadResultWithoutPagingAsync(query, input.LoadOptions)).Items.ToList();
            return _transactionExcelExporter.ExportToFile(data);

        }
    }
}
