using System;

namespace TACHYON.Integration.WaslIntegration.Modules
{
    [Serializable]
    public class WaslTripRoot
    {
        /// <summary>
        /// The vehicle Sequence Number. 
        ///It’s serial number 4-9 digits can be found in right bottom of the physical vehicle license card.
        /// </summary>
        public string VehicleSequenceNumber { get; set; }

        /// <summary>
        /// Driver Id number (National ID for Saudis, Iqama ID for non-Saudis)
        /// </summary>
        public string DriverIdentityNumber { get; set; }

        /// <summary>
        /// The unique trip number that identifies this trip in your system
        /// </summary>
        public string TripNumber { get; set; }

        /// <summary>
        /// Departure Latitude (The origin)
        /// </summary>
        public double DepartureLatitude { get; set; }

        /// <summary>
        /// Departure Longitude (The origin
        /// </summary>
        public double DepartureLongitude { get; set; }

        /// <summary>
        /// Expected Destination Latitude (Final Destination)
        /// </summary>
        public double ExpectedDestinationLatitude { get; set; }

        /// <summary>
        /// Expected Destination Latitude (Final Destination)
        /// </summary>
        public double ExpectedDestinationLongitude { get; set; }

        /// <summary>
        /// Departure date
        /// </summary>
        public string DepartedWhen { get; set; }
    }
}