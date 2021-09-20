using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.AddressBook;
using TACHYON.Authorization.Users;
using TACHYON.MultiTenancy;
using TACHYON.Receivers;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Rating
{
    [Table("RatingLogs")]
    public class RatingLog : Entity<long>, IHasCreationTime
    {
        public int? ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public Receiver ReceiverFk { get; set; }
        public long? DriverId { get; set; }
        [ForeignKey("DriverId")]
        public User DriverFk { get; set; }
        public int? ShipperId{get; set;}
        [ForeignKey("ShipperId")]
        public Tenant ShipperFk { get; set; }
        public int? CarrierId { get; set; }
        [ForeignKey("CarrierId")]
        public Tenant CarrierFk { get; set; }
        public long? PointId { get; set; }
        [ForeignKey("PointId")]
        public RoutPoint RoutePointFk { get; set; }
        public int? TripId { get; set; }
        [ForeignKey("TripId")]
        public ShippingRequestTrip TripFk { get; set; }
        public long? FacilityId { get; set; }
        [ForeignKey("FacilityId")]
        public Facility FacilityFk { get; set; }
        public RateType RateType { get; set; }
        public decimal Rate { get; set; }
        public string Note { get; set; }
        public DateTime CreationTime { get; set; }
        /// <summary>
        /// This field is for receiver
        /// </summary>
        public string Code { get; set; }
    }
}
