using System;

namespace TACHYON.Integration.WaslIntegration.Modules
{
    [Serializable]
    public class WaslTripUpdateRoot
    {
        /// <summary>
        /// Actual Destination Latitude (Final Destination)
        /// </summary>
        public double ActualDestinationLatitude { get; set; }

        /// <summary>
        /// Actual Destination Longitude (Final Destination)
        /// </summary>
        public double ActualDestinationLongitude { get; set; }

        /// <summary>
        /// Arrival date & Time
        /// </summary>
        public string ArrivedWhen { get; set; }
    }
}