using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Cities;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Trucks;

namespace TACHYON.DynamicInvoices.DynamicInvoiceItems
{
    [Table("DynamicInvoiceItems")]
    public class DynamicInvoiceItem : FullAuditedEntity<long>
    {

        public int? TripId { get; set; }

        [ForeignKey(nameof(TripId))]
        public ShippingRequestTrip ShippingRequestTrip { get; set; }
        
        public string Description { get; set; }

        public decimal Price { get; set; }

        public int? OriginCityId { get; set; }

        [ForeignKey(nameof(OriginCityId))]
        public City OriginCity { get; set; }

        public int? DestinationCityId { get; set; }

        [ForeignKey(nameof(DestinationCityId))]
        public City DestinationCity { get; set; }

        public long? TruckId { set; get; }

        [ForeignKey(nameof(TruckId))]
        public Truck Truck { get; set; }

        public DateTime? WorkDate { get; set; }

        public int? Quantity { get; set; }

        public string ContainerNumber { get; set; }
        

        /// <summary>
        /// All Dynamic invoice Items is grouped by DynamicInvoiceId
        /// <remarks>DynamicInvoice `Root` Id</remarks>
        /// </summary>
        public long DynamicInvoiceId { get; set; }

        [ForeignKey(nameof(DynamicInvoiceId))]
        public DynamicInvoice DynamicInvoice { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }

    }
}