using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Invoices
{
    public class InvoiceNoteItem : Entity<long>
    {
        public long InvoiceNoteId { get; set; }
        [ForeignKey(nameof(InvoiceNoteId))] 
        public InvoiceNote InvoiceNoteFK { get; set; }
        public int TripId { get; set; }
        [ForeignKey(nameof(TripId))] 
        public ShippingRequestTrip ShippingRequestTripFK { get; set; }
    }
}
