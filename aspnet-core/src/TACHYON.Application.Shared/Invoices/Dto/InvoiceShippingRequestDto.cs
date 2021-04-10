using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.Dto
{
   public class InvoiceShippingRequestDto: IHasCreationTime
    {
        public decimal Price { get; set; }
        public string TruckType { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
