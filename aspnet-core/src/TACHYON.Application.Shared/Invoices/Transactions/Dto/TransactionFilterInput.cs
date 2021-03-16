using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.Transactions.Dto
{
    public class TransactionFilterInput : PagedAndSortedResultRequestDto
    {
        public ChannelType? channelType { get; set; }
        public int? TenantId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }


    }
}
