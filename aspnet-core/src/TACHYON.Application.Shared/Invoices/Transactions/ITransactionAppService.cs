using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Invoices.Transactions.Dto;

namespace TACHYON.Invoices.Transactions
{
  public  interface ITransactionAppService: IApplicationService
    {
       Task<PagedResultDto<TransactionListDto>> GetAll(TransactionFilterInput input);

    }
}
