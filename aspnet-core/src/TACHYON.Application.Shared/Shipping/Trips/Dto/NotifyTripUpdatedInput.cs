// unset

using Abp;

namespace TACHYON.Shipping.Trips.Dto
{
    public class NotifyTripUpdatedInput
    {
        public int CarrierTenantId { get; set; }

        public int ShipperTenantId { get; set; }

        public UserIdentifier DriverIdentifier { get; set; }

        public int TripId { get; set; }

        public string WaybillNumber { get; set; }

        public string UpdatedBy { get; set; }
    }
}