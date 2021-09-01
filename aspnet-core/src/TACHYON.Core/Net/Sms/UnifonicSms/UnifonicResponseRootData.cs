// unset

namespace TACHYON.Net.Sms.UnifonicSms
{
    public class UnifonicResponseRootData
    {
        public long MessageID { get; set; }
        public string CorrelationID { get; set; }
        public string Status { get; set; }
        public int NumberOfUnits { get; set; }
        public int Cost { get; set; }
        public int Balance { get; set; }
        public string Recipient { get; set; }
        public string TimeCreated { get; set; }
        public string CurrencyCode { get; set; }
    }
}