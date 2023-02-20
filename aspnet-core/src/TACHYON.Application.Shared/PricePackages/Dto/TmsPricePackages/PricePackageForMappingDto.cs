namespace TACHYON.PricePackages.Dto.TmsPricePackages
{
    public class PricePackageForMappingDto
    {
        public TmsPricePackageForViewDto PricePackage { get; set; }

        public bool HasOffer { get; set; }

        public bool HasDirectRequest { get; set; }

        public bool IsRequestPriced { get; set; }

        public bool HasParentOffer { get; set; }
    }
}