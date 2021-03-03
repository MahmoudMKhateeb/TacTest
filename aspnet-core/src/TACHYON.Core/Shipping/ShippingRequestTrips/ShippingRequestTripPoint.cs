using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TACHYON.Shipping.ShippingRequestTrips
{
    [Table("ShippingRequestTripsPoints")]
    public class ShippingRequestTripPoint : Entity
    {
        public int TripId { get; set; }
        [ForeignKey("TripId")]
        public ShippingRequestTrip Trip { get; set; }
        public long PointId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public bool IsActive { get; set; }
        public bool IsComplete { get; set; }

        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }

        public string Code { get; set; }

        public int? Rating { get; set; }

    }
}
