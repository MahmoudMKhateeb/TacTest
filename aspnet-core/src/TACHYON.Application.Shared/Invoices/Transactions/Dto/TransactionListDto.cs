using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.Transactions.Dto
{
    public class TransactionListDto : EntityDto<long>, IHasCreationTime
    {
        public DateTime CreationTime { get; set ; }
        public string ClientName { get; set; }
        public string Edition { get; set; }
        public byte ChannelId { get; set; }
        public string Channel { get; set; }
        public decimal Amount { get; set; }
        public int Count { get; set; }

        public long SourceId { get; set; }

    }
}
