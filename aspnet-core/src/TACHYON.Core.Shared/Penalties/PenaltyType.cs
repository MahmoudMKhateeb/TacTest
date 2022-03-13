using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Penalties
{
   public enum PenaltyType : byte
    {
        NotLogged,
        ShippingRequestCancelingDuringPostPriceProcess,
        ShippingRequestCancelBeforeCompletionTrips,
        TripCancelingBeforeDeliveringAllDrops,
        DetentionPeriodExceedMaximumAllowedTime,
        NotAssigningTruckAndDriverBeforeTheDateForTheTrip,
        NotDeliveringAllDropsBeforeExpectedTripEndDate
    }
}
