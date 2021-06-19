using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Invoices.SubmitInvoices
{
    [Table("SubmitInvoiceTrips")]
  public  class SubmitInvoiceTrip: Entity<long>
    {
        public long SubmitId { get; set; }
        [ForeignKey(nameof(SubmitId))]
        public SubmitInvoice SubmitInvoicesFK { get; set; }
        public int TripId { get; set; }
        [ForeignKey(nameof(TripId))]
        public ShippingRequestTrip ShippingRequestTripFK { get; set; }
    }
}
