namespace TACHYON.BayanIntegration.Modules
{
    public class Vehicle
    {
        /// <summary>
        /// Vehicle’s sequence number
        /// Istemara number in tachyon 
        /// </summary>
        public string SequenceNumber { get; set; }
        public string PlateType { get; set; }
        /// <summary>
        /// Vehicle’s plate
        /// rightLetter + middleLetter + leftLetter + umber
        /// </summary>
        public VehiclePlate VehiclePlate { get; set; }
    }
}