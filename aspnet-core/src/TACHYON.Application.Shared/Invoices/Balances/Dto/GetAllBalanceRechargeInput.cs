using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.Balances.Dto
{
    public class GetAllBalanceRechargeInput: PagedAndSortedResultRequestDto
    {
        public int? TenantId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }


    }
}
