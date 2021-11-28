namespace TACHYON.Routs.RoutPoints
{
    public static class RoutPointConsts
    {
        public const string Step1 = "StartMovingToLoadingLocation";
        public const string Step2 = "ArriveToLoadingLocation";
        public const string Step3 = "FinishLoading";
        public const string Step4 = "StartMovingToOffloadingLocation";
        public const string Step5 = "ArrivedToOffloadingLocation";
        public const string Step6 = "StartOffloadingShipment";
        public const string Step7 = "FinishOffloadingShipment";
        public const string Step8 = "DeliveryConfirmed";

        public const string Action1 = "DriverIsChanged";
        public const string Action2 = "TruckIsChanged";
        public const string Action3 = "IncidentHappened";
    }
}