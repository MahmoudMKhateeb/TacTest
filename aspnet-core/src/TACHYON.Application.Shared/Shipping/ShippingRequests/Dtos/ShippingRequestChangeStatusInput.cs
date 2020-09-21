namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class ShippingRequestChangeStatusInput
    {
        public long Id { get; set; }
        public int ShippingRequestStatusId { get; set; }
    }
}