﻿using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.AddressBook;
using TACHYON.Authorization.Users;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.Trips;
using TACHYON.Shipping.TripStatuses;
using TACHYON.ShippingRequestTripVases;
using TACHYON.Trucks;

namespace TACHYON.Shipping.ShippingRequestTrips
{
    [Table("ShippingRequestTrips")]
    public class ShippingRequestTrip : FullAuditedEntity
    {
        public DateTime StartTripDate { get; set; }
        public DateTime EndTripDate { get; set; }
        public DateTime? StartWorking { get; set; }
        public DateTime? EndWorking { get; set; }
        public ShippingRequestTripStatus Status { get; set; } = ShippingRequestTripStatus.StandBy;

        public long? AssignedDriverUserId { get; set; }
        [ForeignKey("AssignedDriverUserId")]
        public User AssignedDriverUserFk { get; set; }
        public long? AssignedTruckId { get; set; }
        [ForeignKey("AssignedTruckId")]
        public Truck AssignedTruckFk { get; set; }
        public long ShippingRequestId { get; set; }
        [ForeignKey("ShippingRequestId")]
        public ShippingRequest ShippingRequestFk { get; set; }


        //Facility
        public virtual long? OriginFacilityId { get; set; }

        [ForeignKey("OriginFacilityId")]
        public Facility OriginFacilityFk { get; set; }

        public virtual long? DestinationFacilityId { get; set; }

        [ForeignKey("DestinationFacilityId")]
        public Facility DestinationFacilityFk { get; set; }

        public ICollection<RoutPoint> RoutPoints { get; set; }
        public ICollection<ShippingRequestTripVas> ShippingRequestTripVases { get; set; }
    }
}