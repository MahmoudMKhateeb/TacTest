using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TACHYON.Penalties
{
   public enum PenaltyType : byte
    {
        [Description("NotLogged")]
        NotLogged,
        [Description("TripCancelation")]
        TripCancelation,
        [Description("ExceedMaximumAllowedTime")]
        DetentionPeriodExceedMaximumAllowedTime,
        [Description("TruckAndDriverLateAssigned")]
        NotAssigningTruckAndDriverBeforeTheDateForTheTrip,
        [Description("DropsDeliveringAfterTripEndDate")]
        NotDeliveringAllDropsBeforeExpectedTripEndDate,
    }
}
