using Abp.Domain.Entities;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TACHYON.Routs.RoutPoints
{
    [Table("RoutePointTransitions")]
    public  class RoutePointTransition:Entity
    {
        public long? FromPointId { get; set; }
        [ForeignKey("FromPointId")]
        public RoutPoint FromPoint { get; set; }
        public Point FromLocation { get; set; }

        public long ToPointId { get; set; }
        [ForeignKey("ToPointId")]
        public RoutPoint ToPoint { get; set; }
        public Point ToLocation { get; set; }

        public bool IsComplete { get; set; }

    }
}
