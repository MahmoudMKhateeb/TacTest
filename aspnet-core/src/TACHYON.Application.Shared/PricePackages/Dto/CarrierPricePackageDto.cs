namespace TACHYON.PricePackages.Dto
{
    public class CarrierPricePackageDto
    {
        public long PricePackageId { get; set; }
        
        public string PricePackageReference { get; set; }
        
        public int CarrierTenantId { get; set; }
    }
}