namespace TACHYON.PricePackages.Dto.NormalPricePackage
{
    public class PricePackagesPricingInfoDto
    {
        public string TruckTypeName { get; set; }
        public decimal DirectRequestMinPrice { get; set; }

        /// <summary>
        /// The name of tenant that is submit the min price of direct request
        /// Note: usually the tenant edition is `CARRIER`
        /// </summary>
        public string DirectRequestMinPriceOwner { get; set; }

        public decimal DirectRequestAveragePrice { get; set; }

        public decimal DirectRequestMaxPrice { get; set; }
        
        /// <summary>
        /// The name of tenant that is submit the max price of direct request
        /// Note: usually the tenant edition is `CARRIER`
        /// </summary>
        public string DirectRequestMaxPriceOwner { get; set; }
        
        public decimal TmsMinPrice { get; set; }

        /// <summary>
        /// The name of tenant that is submit the min price of tachyon manage
        /// Note: usually the tenant edition is `CARRIER`
        /// </summary>
        public string TmsMinPriceOwner { get; set; }

        public decimal TmsAveragePrice { get; set; }

        public decimal TmsMaxPrice { get; set; }
        
        /// <summary>
        /// The name of tenant that is submit the max price of tachyon manage
        /// Note: usually the tenant edition is `CARRIER`
        /// </summary>
        public string TmsMaxPriceOwner { get; set; }
    }
}