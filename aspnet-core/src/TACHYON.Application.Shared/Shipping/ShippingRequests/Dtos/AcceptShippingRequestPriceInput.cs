namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class AcceptShippingRequestPriceInput
    {
        public AcceptShippingRequestPriceInput(long id, bool accept)
        {
            Id = id;
            Accept = accept;
        }

        public long Id { get; private set; }
        public bool Accept { get; private set; }
    }
}