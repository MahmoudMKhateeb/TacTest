using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TACHYON.Shipping.Dedicated
{
    [Table("DedicatedShippingRequestTruckAttendances")]
    public class DedicatedShippingRequestTruckAttendance: FullAuditedEntity<long>
    {
        public long DedicatedShippingRequestTruckId { get; set; }
        [ForeignKey("DedicatedShippingRequestTruckId")]
        public DedicatedShippingRequestTruck DedicatedShippingRequestTruck { get; set; }
        public DateTime AttendanceDate { get; set; }
        public AttendaceStatus AttendaceStatus { get; set; }
    }
}
