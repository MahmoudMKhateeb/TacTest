namespace TACHYON.PricePackages.Dto
{
    public class PricePackageForMappingDto
    {
        public PricePackageForViewDto PricePackage { get; set; }

        public bool HasOffer { get; set; }

        public bool HasDirectRequest { get; set; }

        public bool IsRequestPriced { get; set; }

        public bool HasParentOffer { get; set; }
    }
}