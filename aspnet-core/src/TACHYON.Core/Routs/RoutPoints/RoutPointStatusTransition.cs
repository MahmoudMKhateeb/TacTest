using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Routs.RoutPoints;

namespace TACHYON.Shipping.RoutPoints
{
    [Table("RoutPointStatusTransitions")]
    public class RoutPointStatusTransition : Entity, IHasCreationTime
    {
        public long PointId { get; set; }
        [ForeignKey("PointId")]
        public RoutPoint RoutPointFK { get; set; }
        public RoutePointStatus Status { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsReset { get; set; }

    }
}