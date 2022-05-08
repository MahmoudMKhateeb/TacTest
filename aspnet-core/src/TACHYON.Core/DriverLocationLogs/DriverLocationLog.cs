using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Authorization.Users;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.DriverLocationLogs
{
    [Table("DriverLocationLogs")]
    public class DriverLocationLog : Entity<long>, ICreationAudited
    {
        public long? CreatorUserId { set; get; }
        [ForeignKey("CreatorUserId")] public User CreatorUserFk { get; set; }
        public DateTime CreationTime { set; get; }
        public int? TripId { get; set; }
        [ForeignKey("TripId")] public ShippingRequestTrip ShippingRequestTripFk { get; set; }
        public Point Location { get; set; }
    }
}