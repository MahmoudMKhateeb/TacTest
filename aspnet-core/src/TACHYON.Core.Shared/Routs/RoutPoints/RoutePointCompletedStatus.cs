using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TACHYON.Routs.RoutPoints
{
    public enum RoutePointCompletedStatus : byte
    {
        /// <summary>
        /// when point status is before finishoffLoadShipment, default status
        /// </summary>
        NotCompleted = 0,
        /// <summary>
        /// When point finishOffLoadShipment and receiver code not confirmed
        /// </summary>
        [Description("WhenPointFinishOffLoadAndReceiverCodeNotConfirmed")]
        CompletedAndMissingReceiverCode = 1,
        /// <summary>
        /// When point ReceiverConfirmation and POD not uploaded
        /// </summary>
        CompletedAndMissingPOD = 2,
        Completed = 3
    }
}