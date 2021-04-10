using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Invoices.SubmitInvoices;

namespace TACHYON.Invoices.Groups.Dto
{
   public class GroupPeriodFilterInput: PagedAndSortedResultRequestDto
    {
        public int? TenantId { get; set; }
        public int? PeriodId { get; set; }


      //  public bool? IsDemand { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
        public SubmitInvoiceStatus? Status { get; set; }


    }
}
