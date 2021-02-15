using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.Dto
{
  public  class InvoiceFilterInput: PagedAndSortedResultRequestDto
    {
        public string ClientName { get; set; }
        public bool? IsPaid { get; set; }

        public bool? IsAccountReceivable { get; set; }

    }
}
