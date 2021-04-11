using System;

namespace TACHYON.Mobile.Dtos
{
    public class UserDeviceTokenDto
    {
        public long UserId { get; set; }
        public string DeviceId { get; set; }
        public string Token { get; set; }
        public DateTime? ExpireDate { get; set; }

    }
}
