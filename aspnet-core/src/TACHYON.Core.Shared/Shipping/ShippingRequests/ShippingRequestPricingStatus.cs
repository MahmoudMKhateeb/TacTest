namespace TACHYON.Shipping.ShippingRequests
{
    public enum ShippingRequestPricingStatus
    {
        New=0,
        Accepted = 1, //accepted and there is assigned carrier
        Rejected = 2,
        AcceptedAndWaitingForCarrier = 3
    }
}
