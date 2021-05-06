namespace TACHYON.Routs.RoutPoints
{
    public enum RoutePointStatus : byte
    {
        StandBy=0,
        StartedMovingToLoadingLocation = 1,
        ArriveToLoadingLocation = 2,
        StartLoading = 3,
        FinishLoading=4,
        StartedMovingToOfLoadingLocation = 5,
        ArrivedToDestination =6,
        StartOffloading =7,
        FinishOffLoadShipment = 8,
        ReceiverConfirmed =9,
        DeliveryConfirmation = 10,
        Issue=11
    }
}
