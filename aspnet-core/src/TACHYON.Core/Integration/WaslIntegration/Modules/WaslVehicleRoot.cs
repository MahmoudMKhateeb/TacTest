using System;

namespace TACHYON.Integration.WaslIntegration.Modules
{
    [Serializable]
    public class WaslVehicleRoot
    {
        /// <summary>
        /// The vehicle Sequence Number. It’s serial number 4-9 digits can be foundin right bottom of the physical vehicle license card.
        /// </summary>
        public string SequenceNumber { get; set; }

        /// <summary>
        /// Vehicle plate number. Must follow the format
        /// ا ا ا 1234
        /// letters first and then numbers. Each letter 
        /// should be separated by a space and a single
        /// space between letter and number
        /// </summary>
        public string Plate { get; set; }

        /// <summary>
        /// Refer to appendix for plate types
        /// </summary>
        public int PlateType { get; set; }
    }
}