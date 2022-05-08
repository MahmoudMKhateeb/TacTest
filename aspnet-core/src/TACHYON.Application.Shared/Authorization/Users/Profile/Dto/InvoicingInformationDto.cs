namespace TACHYON.Authorization.Users.Profile.Dto
{
    public class InvoicingInformationDto
    {
        public string CreditLimit { get; set; }

        public string CurrentBalance { get; set; }

        public string InvoicingFrequency { get; set; }

        public string CreditType { get; set; }

        public int InvoicingDuePeriod { get; set; }
    }
}