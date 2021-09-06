// unset

using System;

namespace TACHYON.Net.Sms.UnifonicSms
{
    [Serializable]
    public class UnifonicSendSmsJobArgs
    {
        public string Recipient { get; set; }
        public string Text { get; set; }
    }
}