// unset

namespace TACHYON.Net.Sms.UnifonicSms
{
    public class UnifonicResponseRoot
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string errorCode { get; set; }
        public UnifonicResponseRootData data { get; set; }
    }
}