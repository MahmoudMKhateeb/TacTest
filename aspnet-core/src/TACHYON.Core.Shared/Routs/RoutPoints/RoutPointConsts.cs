namespace TACHYON.Routs.RoutPoints
{
    public static class RoutPointConsts
    {
        public const string PickUpStep1 = "StartMovingToLoadingLocation";
        public const string PickUpStep2 = "ArriveToLoadingLocation";
        public const string PickUpStep3 = "StartLoading";
        public const string PickUpStep4 = "FinishLoading";
        public const string DropOffStep1 = "StartMovingToOffloadingLocation";
        public const string DropOffStep2 = "ArrivedToOffloadingLocation";
        public const string DropOffStep3 = "StartOffloadingShipment";
        public const string DropOffStep4 = "FinishOffloadingShipment";
        public const string DropOffStep5 = "ReceiverConfirmed";
        public const string DropOffStep6 = "DeliveryConfirmed";

        public const string Action1 = "DriverIsChanged";
        public const string Action2 = "TruckIsChanged";
        public const string Action3 = "IncidentHappened";
    }
}