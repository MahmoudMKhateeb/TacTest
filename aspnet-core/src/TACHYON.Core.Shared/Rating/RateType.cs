using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TACHYON.Rating
{
    public enum RateType: byte
    {
        [Description("ReceiverRatesDriverInPoint")]
        DriverByReceiver=1,

        [Description("ReceiverRatesDeliveryEexperienceInPoint")]
        DEByReceiver =2,

        [Description("ShipperRatesCarrierInTrip")]
        CarrierByShipper =3,

        [Description("DriverRatesFacilityInPoint")]
        FacilityByDriver =4,

        [Description("DriverRatesShippingExperienceInTrip")]
        SEByDriver =5,

        [Description("CarrierRatesShipperInTrip")]
        ShipperByCarrier =6,

        [Description("SystemRecalculateShipperTripRatingAfterEachPointRateInsert")]
        ShipperTripBySystem = 7,

        [Description("SystemRecalculateCarrierTripRatingAfterEachPointOrTripRateInsert")]
        CarrierTripBySystem = 8

    }
}
