using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Invoices
{
    [Table("InvoiceTrips")]
  public  class InvoiceTrip:Entity<long>
    {
        public long InvoiceId { get; set; }
        [ForeignKey(nameof(InvoiceId))]
        public Invoice InvoiceFK { get; set; }
        public int TripId { get; set; }
        [ForeignKey(nameof(TripId))]
        public ShippingRequestTrip ShippingRequestTripFK { get; set; }
    }
}
