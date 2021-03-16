using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TACHYON.Routs.RoutPoints
{
    [Table("RoutePointTransitions")]
    public  class RoutePointTransition:Entity
    {
        public long FromPointId { get; set; }
        [ForeignKey("FromPointId")]
        public RoutPoint FromPoint { get; set; }
        public long? ToPointId { get; set; }
        [ForeignKey("ToPointId")]
        public RoutPoint ToPoint { get; set; }

    }
}
