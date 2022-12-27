using System;

namespace TACHYON.Shipping.Trips.Importing.Dto
{
    [Serializable]
    public class ForceDeliverTripJobArgs
    {
        public Guid BinaryObjectId { get; set; }

        public long RequestedByUserId { get; set; }
        
        public int? RequestedByTenantId { get; set; }
        
    }
}