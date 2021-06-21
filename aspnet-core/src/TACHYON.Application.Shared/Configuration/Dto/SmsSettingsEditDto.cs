using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Configuration.Dto
{
    public class SmsSettingsEditDto
    {
        public string UnifonicAppSid { get; set; }
        public string UnifonicSenderId { get; set; }
        public string UnifonicAdvertisingSenderId { get; set; }
        public string UnifonicNotificationSenderId { get; set; }
    }
}
