using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Trips.Dto
{
    public  class ShippingRequestTripDto
    {
        public DateTime StartTripDate { get; set; }
        public DateTime EndTripDate { get; set; }
        public DateTime? StartWorking { get; set; }
        public DateTime? EndWorking { get; set; }
        public byte StatusId { get; set; }
        public long? AssignedDriverUserId { get; set; }
        public long? AssignedTruckId { get; set; }
        public long ShippingRequestId { get; set; }

        public string Code { get; set; }

        //Facility
        public virtual long? OriginFacilityId { get; set; }
        public virtual long? DestinationFacilityId { get; set; }
    }
}
