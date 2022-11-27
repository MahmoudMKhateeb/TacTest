using TACHYON.Shipping.ShippingRequestTrips;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Integration.BayanIntegration
{
    [Table("BayanIntegrationResults")]
    public class BayanIntegrationResult : CreationAuditedEntity<long>
    {

        [Required]
        public virtual string ActionName { get; set; }

        [Required]
        public virtual string InputJson { get; set; }

        public virtual string ResponseJson { get; set; }

        public virtual string Version { get; set; }

        public virtual int ShippingRequestTripId { get; set; }

        [ForeignKey("ShippingRequestTripId")]
        public ShippingRequestTrip ShippingRequestTripFk { get; set; }

    }
}