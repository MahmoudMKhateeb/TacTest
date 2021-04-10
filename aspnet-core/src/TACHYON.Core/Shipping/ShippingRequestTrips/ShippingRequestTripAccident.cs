using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TACHYON.Common;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.Accidents;
using NetTopologySuite.Geometries;

namespace TACHYON.Shipping.ShippingRequestTrips
{
    [Table("ShippingRequestTripAccidents")]
    public class ShippingRequestTripAccident : FullAuditedEntity,IHasDocument
    {
        public long PointId { get; set; }
        [ForeignKey("PointId")]
        public RoutPoint RoutPointFK { get; set; }
        public int? ReasoneId { get; set; }
        [ForeignKey("ReasoneId")]
        public ShippingRequestReasonAccident ResoneFK { get; set; }

        [StringLength(500,MinimumLength =10)]
        public string Description {get;set; }
        public bool IsResolve { get; set; }
        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }

        public Point Location { get; set; }


    }

}
