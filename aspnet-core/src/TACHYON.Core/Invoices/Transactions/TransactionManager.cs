using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TACHYON.Invoices.Transactions
{
   public class TransactionManager: TACHYONDomainServiceBase
    {
        private readonly IRepository<Transaction, long> _TransactionRepository;
        public TransactionManager(IRepository<Transaction, long> TransactionRepository)
        {
            _TransactionRepository = TransactionRepository;
        }

        public async Task Create(Transaction transaction)
        {
            await _TransactionRepository.InsertAsync(transaction);
        }

        public async Task Delete(long SourceId,ChannelType channelType)
        {
            var transaction = await _TransactionRepository.SingleAsync(t => t.SourceId == SourceId && t.ChannelId == (byte)channelType);
            await _TransactionRepository.DeleteAsync(transaction);
        }
    }
}
