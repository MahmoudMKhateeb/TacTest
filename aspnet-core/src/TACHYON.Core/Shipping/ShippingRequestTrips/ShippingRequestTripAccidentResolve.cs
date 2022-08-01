using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Common;
using TACHYON.Shipping.Trips;

namespace TACHYON.Shipping.ShippingRequestTrips
{
    [Table("ShippingRequestTripAccidentResolves")]
    public class ShippingRequestTripAccidentResolve : FullAuditedEntity
    {
        public int AccidentId { get; set; }
        
        [ForeignKey("AccidentId")] 
        public ShippingRequestTripAccident AccidentFk { get; set; }
        
        [Required]
        public TripAccidentResolveType ResolveType { get; set; }
        public long? DriverId { get; set; }
        public long? TruckId { get; set; }
        public bool IsApplied { get; set; }
        
        [DefaultValue(false)]
        public bool ApprovedByShipper { get; set; }
        
        [DefaultValue(false)]
        public bool ApprovedByCarrier { get; set; }

    }
}