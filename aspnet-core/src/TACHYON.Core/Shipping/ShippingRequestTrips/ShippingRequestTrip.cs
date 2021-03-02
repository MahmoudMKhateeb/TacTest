using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Authorization.Users;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.TripStatuses;
using TACHYON.Trucks;

namespace TACHYON.Shipping.ShippingRequestTrips
{
    [Table("ShippingRequestTrips")]
    public class ShippingRequestTrip : FullAuditedEntity
    {
        public DateTime StartTripDate { get; set; }
        public DateTime EndTripDate { get; set; }
        public int TripStatusId { get; set; }
        [ForeignKey("TripStatusId")]
        public TripStatus TripStatusFk { get; set; }

        public long? AssignedDriverUserId { get; set; }
        [ForeignKey("AssignedDriverUserId")]
        public User AssignedDriverUserFk { get; set; }
        public long? AssignedTruckId { get; set; }
        [ForeignKey("AssignedTruckId")]
        public Truck AssignedTruckFk { get; set; }
        public long ShippingRequestId { get; set; }
        [ForeignKey("ShippingRequestId")]
        public ShippingRequest ShippingRequestFk { get; set; }

        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }

    }
}
