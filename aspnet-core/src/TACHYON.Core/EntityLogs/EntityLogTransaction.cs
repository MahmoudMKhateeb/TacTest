using TACHYON.EntityLogs.Transactions;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutPoints.RoutPointSmartEnum;
using TACHYON.SmartEnums;

namespace TACHYON.EntityLogs
{
    public abstract class EntityLogTransaction : SmartEnum
    {
        
        // Tracking Transactions Value From 0 To 49 (min/max)
        // ShippingRequest Transactions Value From 50 To 79 (min/max)
        // Notice :-
        // => if you want to add a new transaction please set your transactions value start from last transaction
        // for example shipping request transactions max value 79 then you must start from 80 and so on
        // and please add comment here if you wanna add new transactions to explain what it is 

        #region TrackingTransactions

        public static readonly EntityLogTransaction RoutPointPickUpStep1 = new RoutPointPickUpStep1(nameof(RoutPointPickUpStep1), (int)RoutePointStatus.StartedMovingToLoadingLocation);
        public static readonly EntityLogTransaction RoutPointPickUpStep2 = new RoutPointPickUpStep2(nameof(RoutPointPickUpStep2), (int)RoutePointStatus.ArriveToLoadingLocation);
        public static readonly EntityLogTransaction RoutPointPickUpStep3 = new RoutPointPickUpStep3(nameof(RoutPointPickUpStep3), (int)RoutePointStatus.StartLoading);
        public static readonly EntityLogTransaction RoutPointPickUpStep4 = new RoutPointPickUpStep4(nameof(RoutPointPickUpStep4), (int)RoutePointStatus.FinishLoading);

        public static readonly EntityLogTransaction RoutPointDropOffStep1 = new RoutPointDropOffStep1(nameof(RoutPointDropOffStep1), (int)RoutePointStatus.StartedMovingToOffLoadingLocation);
        public static readonly EntityLogTransaction RoutPointDropOffStep2 = new RoutPointDropOffStep2(nameof(RoutPointDropOffStep2), (int)RoutePointStatus.ArrivedToDestination);
        public static readonly EntityLogTransaction RoutPointDropOffStep3 = new RoutPointDropOffStep3(nameof(RoutPointDropOffStep3), (int)RoutePointStatus.StartOffloading);
        public static readonly EntityLogTransaction RoutPointDropOffStep4 = new RoutPointDropOffStep4(nameof(RoutPointDropOffStep4), (int)RoutePointStatus.FinishOffLoadShipment);
        public static readonly EntityLogTransaction RoutPointDropOffStep5 = new RoutPointDropOffStep5(nameof(RoutPointDropOffStep5), (int)RoutePointStatus.ReceiverConfirmed);
        public static readonly EntityLogTransaction RoutPointDropOffStep6 = new RoutPointDropOffStep6(nameof(RoutPointDropOffStep6), (int)RoutePointStatus.DeliveryConfirmation);
        public static readonly EntityLogTransaction RoutPointDropOffStep7 = new RoutPointDropOffStep7(nameof(RoutPointDropOffStep7), (int)RoutePointStatus.DeliveryNoteUploded);
        public static readonly EntityLogTransaction RoutPointDropOffStep8 = new RoutPointDropOffStep8(nameof(RoutPointDropOffStep8), (int)RoutePointStatus.UplodeGoodPicture);

        public static readonly EntityLogTransaction RoutPointAction1 = new RoutPointAction1(nameof(RoutPointAction1), 9); 
        public static readonly EntityLogTransaction RoutPointAction2 = new RoutPointAction2(nameof(RoutPointAction2), 10);
        public static readonly EntityLogTransaction RoutPointAction3 = new RoutPointAction3(nameof(RoutPointAction3), 11);
        public static readonly EntityLogTransaction RoutPointAction4 = new RoutPointAction4(nameof(RoutPointAction4), 12);
 

        #endregion

        #region ShippingRequestTransactions

        public static readonly EntityLogTransaction UpdateShippingRequest = new UpdateShippingRequestTransaction(nameof(UpdateShippingRequestTransaction), 50);
        public static readonly EntityLogTransaction CreatePriceOffer = new CreateShippingRequestPriceOfferTransaction(nameof(CreateShippingRequestPriceOfferTransaction), 51);
        public static readonly EntityLogTransaction UpdatePriceOffer = new UpdateShippingRequestPriceOfferTransaction(nameof(UpdateShippingRequestPriceOfferTransaction), 52);
        public static readonly EntityLogTransaction AcceptPriceOffer = new AcceptShippingRequestPriceOfferTransaction(nameof(AcceptShippingRequestPriceOfferTransaction), 53);

        #endregion

        public static readonly EntityLogTransaction DefaultLogTransaction = new DefaultLogTransaction();


        public abstract string Transaction { get; }

        protected EntityLogTransaction(string displayName, int value) : base(displayName, value)
        {
        }
    }

    public class DefaultLogTransaction : EntityLogTransaction
    {
        public DefaultLogTransaction() : base(nameof(DefaultLogTransaction), -1)
        {
        }

        public override string
            Transaction { get => "NoReasonProvided"; } // Without Spaces To Use It As Localized String Key
    }
}