using System;

namespace TACHYON.Integration.WaslIntegration.Modules
{
    [Serializable]
    public class WaslDriversRoot
    {
        /// <summary>
        /// Driver Id number (National ID for Saudis, Iqama ID for non-Saudis)
        /// </summary>
        public string IdentityNumber { get; set; }
        /// <summary>
        /// Either Hijri or Gregorian date is required
        /// </summary>
        public string DateOfBirthHijri { get; set; }
        public string DateOfBirthGregorian { get; set; }
        /// <summary>
        /// Driver email
        /// Optional 
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Driver Mobile Numbe
        /// </summary>
        public string MobileNumber { get; set; }
    }
}