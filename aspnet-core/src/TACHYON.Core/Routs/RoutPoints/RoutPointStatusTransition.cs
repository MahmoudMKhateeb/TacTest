using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.Trips;

namespace TACHYON.Shipping.RoutPoints
{
    [Table("RoutPointStatusTransitions")]
    public class RoutPointStatusTransition : Entity, IHasCreationTime
    {
        public long PointId { get; set; }
        [ForeignKey("PointId")]
        public RoutPoint RoutPointFK { get; set; }
        public ShippingRequestTripStatus Status { get; set; } 
        public DateTime CreationTime { get; set; } 


    }
}
