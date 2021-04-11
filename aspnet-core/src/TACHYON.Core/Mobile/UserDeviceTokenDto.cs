using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Mobile
{
  public  class UserDeviceTokenDto1
    {

        public long UserId { get; set; }
        public string DeviceId { get; set; }
        public string Token { get; set; }
        public DateTime? ExpireDate { get; set; }

    }
}
