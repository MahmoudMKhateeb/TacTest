using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.Dto
{
    public class InvoiceSearchInputDto
    {
        public DateTime? PaymentDate { get; set; }
        public long? WaybillOrSubWaybillNumber { get; set; }
        public string ContainerNumber { get; set; }
        public string AccountNumber { get; set; }

    }
}
