using System;

namespace TACHYON.Web.Models.TokenAuth
{
    public class AuthenticateMobileModel
    {
        public string Username { get; set; }
        public string OTP { get; set; }
        public string Language { get; set; }

        public string DeviceToken { get; set; }
        public string DeviceId { get; set; }
        public DateTime? DeviceExpireDate { get; set; }
    }
}
