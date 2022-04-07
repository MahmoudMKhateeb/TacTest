using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Common;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Shipping.Notes;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.MultiTenancy;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Shipping.ShippingRequestAndTripNotes
{
    [Table("ShippingRequestAndTripNotes")]
    public class ShippingRequestAndTripNote : FullAuditedEntity, IMustHaveTenant
    {
        public string Note { get; set; }
        public int? TripId { get; set; }
        [ForeignKey("TripId")]
        public ShippingRequestTrip TripFK { get; set; }
        public long? ShippingRequetId { get; set; }
        [ForeignKey("ShippingRequetId")]
        public ShippingRequest ShippingRequestFK { get; set; }
        public int TenantId { get; set; }
        [ForeignKey("TenantId")]
        public Tenant TenantFK { get; set; }
        public VisibilityNotes Visibility { get; set; } = VisibilityNotes.Internal;
        public ICollection<DocumentFile> DocumentFiles { get; set; }
        public bool IsPrintedByWabillInvoice { get; set; }
    }
}