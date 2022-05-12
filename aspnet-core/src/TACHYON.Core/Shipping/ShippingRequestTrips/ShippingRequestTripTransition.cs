using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Routs.RoutPoints;

namespace TACHYON.Shipping.ShippingRequestTrips
{
    [Table("ShippingRequestTripTransitions")]
    public class ShippingRequestTripTransition : Entity, IHasCreationTime
    {
        public long? FromPointId { get; set; }
        [ForeignKey("FromPointId")] public RoutPoint FromPoint { get; set; }
        public Point FromLocation { get; set; }

        public long ToPointId { get; set; }
        [ForeignKey("ToPointId")] public RoutPoint ToPoint { get; set; }
        public Point ToLocation { get; set; }

        public bool IsComplete { get; set; }
        public DateTime CreationTime { get; set; } = Clock.Now;
    }
}