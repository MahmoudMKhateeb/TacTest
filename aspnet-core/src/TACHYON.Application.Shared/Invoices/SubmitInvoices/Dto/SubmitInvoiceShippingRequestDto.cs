using Abp.Domain.Entities.Auditing;
using System;

namespace TACHYON.Invoices.SubmitInvoices.Dto
{
    public class SubmitInvoiceShippingRequestDto : IHasCreationTime
    {
        public decimal Price { get; set; }
        public string TruckType { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public DateTime CreationTime { get; set; }
    }
}