namespace TACHYON.PricePackages.Dto
{
    public class PricePackageProfileDto
    {
            public string PricePackageReference { get; set; }
            
            public long Id { get; set; }
            
            public string DisplayName { get; set; }
            
            public string TruckType { get; set; }
            
            public string Origin { get; set; }
            
            public string Destination { get; set; }
            
            public decimal TotalPrice { get; set; }
            
            public int TenantId { get; set; }
    }
}