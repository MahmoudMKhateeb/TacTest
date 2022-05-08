using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Invoices.Transactions.Dto;

namespace TACHYON.Invoices.Transactions
{
    public interface ITransactionAppService : IApplicationService
    {
        Task<LoadResult> GetAll(TransactionFilterInput input);
    }
}