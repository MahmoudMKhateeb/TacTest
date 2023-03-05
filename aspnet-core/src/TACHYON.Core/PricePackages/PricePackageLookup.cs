namespace TACHYON.PricePackages
{
    public class PricePackageLookup
    {
        public PricePackage PricePackage { get; set; }

        public bool HasOffer { get; set; }
        
        public bool HasParentOffer { get; set; }

        public bool HasDirectRequest { get; set; }
    }
}