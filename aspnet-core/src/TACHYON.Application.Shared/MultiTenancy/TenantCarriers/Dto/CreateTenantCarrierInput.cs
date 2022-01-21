namespace TACHYON.MultiTenancy.TenantCarriers.Dto
{
    public class CreateTenantCarrierInput
    {
        public int CarrierTenantId { get; set; }
        public int TenantId { get; set; }
    }
}