namespace TACHYON.Vases.Dtos
{
    public class GetVasPriceForViewDto
    {
        public VasPriceDto VasPrice { get; set; }

        public string VasName { get; set; }

        public bool HasAmount { get; set; }

        public bool HasCount { get; set; }
    }
}