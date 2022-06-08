using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.ShippingRequestTripVases;

namespace TACHYON.Invoices
{
    [Table("InvoiceNoteItems")]
    public class InvoiceNoteItem : Entity<long>
    {
        public long InvoiceNoteId { get; set; }
        [ForeignKey(nameof(InvoiceNoteId))] 
        public InvoiceNote InvoiceNoteFK { get; set; }
        public int? TripId { get; set; }
        [ForeignKey(nameof(TripId))] 
        public ShippingRequestTrip ShippingRequestTripFK { get; set; }
        public long? TripVasId { get; set; }
        [ForeignKey(nameof(TripVasId))]
        public ShippingRequestTripVas ShippingRequestTripVasFK { get; set; }
        /// <summary>
        /// price for invoice note, can be original waybill or vas price, or edited price
        /// </summary>
        public decimal Price { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
