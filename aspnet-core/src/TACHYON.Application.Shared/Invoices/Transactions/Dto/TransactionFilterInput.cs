using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Invoices.Transactions.Dto
{
    public class TransactionFilterInput : PagedAndSortedResultRequestDto
    {
        public ChannelType? channelType { get; set; }
        public int? TenantId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public decimal? minLongitude { get; set; }

        public decimal? maxLongitude { get; set; }

        public int? EditionId { get; set; }


    }
}
