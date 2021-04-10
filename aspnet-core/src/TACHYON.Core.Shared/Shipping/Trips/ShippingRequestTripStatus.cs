namespace TACHYON.Shipping.Trips
{
    public  enum ShippingRequestTripStatus:byte
    {
        StandBy=0,
        InTransitToPickupLocation = 1,
        ReachedPickupLocation = 2,
        Loading = 3,
        InTransitToDropLocation = 4,
        ReachedDropLocation = 5,
        Offloading =6,
        ReceiverConfirmed=7,
        ProofOfDeliveryCompleted = 8,
        Finished = 9,
        Cancled=10,
        Issue=11
    }
}
