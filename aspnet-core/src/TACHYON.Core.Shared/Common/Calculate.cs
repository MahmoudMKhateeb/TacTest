namespace TACHYON.Common
{
    public static class Calculate
    {
        public static decimal CalculateVat(decimal amount, decimal vat)
        {
            return (amount / 100) * vat;
        }
    }
}
