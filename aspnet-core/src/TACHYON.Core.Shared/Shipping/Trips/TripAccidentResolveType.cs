using System.ComponentModel;

namespace TACHYON.Shipping.Trips
{
    public enum TripAccidentResolveType
    {
        [Description("ChangeDriverResolve")]
        ChangeDriver,
        [Description("ChangeTruckResolve")]
        ChangeTruck,
        [Description("ChangeDriverAndTruckResolve")]
        ChangeDriverAndTruck,
        [Description("NoActionNeededResolve")]
        NoActionNeeded,
        CancelTrip,
    }
}