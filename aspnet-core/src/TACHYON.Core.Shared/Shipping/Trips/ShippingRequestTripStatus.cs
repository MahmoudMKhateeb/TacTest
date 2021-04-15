namespace TACHYON.Shipping.Trips
{
    public  enum ShippingRequestTripStatus:byte
    {
        StandBy=0,
        StartedMovingToLoadingLocation = 1,
        ArriveToLoadingLocation = 2,
        StartLoading = 3,
        StartedMovingToOfLoadingLocation = 4,
        ArrivedToDestination = 5,
        StartOffloading =6,
        ReceiverConfirmed=7,
        DeliveryConfirmation = 8,
        Finished = 9,
        FinishLoading=10,
        FinishOffLoadShipment=11,
        Cancled =12,
        Issue=13
    }
}
