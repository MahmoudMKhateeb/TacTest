using System;

namespace TACHYON.Shipping.Trips.Importing.Dto
{
    [Serializable]
    public class ForceDeliverTripInput
    {
        public Guid BinaryObjectId { get; set; }

        public long RequestedByUserId { get; set; }
        
        public int? RequestedByTenantId { get; set; }
        
    }
}