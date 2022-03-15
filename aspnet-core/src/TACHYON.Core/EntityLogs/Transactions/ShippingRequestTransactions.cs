namespace TACHYON.EntityLogs.Transactions
{
    public class UpdateShippingRequestTransaction : EntityLogTransaction
    {
        public UpdateShippingRequestTransaction(string displayName, int value) : base(displayName, value)
        {
        }

        public override string Transaction { get => "ShippingRequestUpdatedTransaction" ; } // Without Spaces To Use It As Localized String Key
    }
    
    public class CreateShippingRequestPriceOfferTransaction : EntityLogTransaction
    {
        public CreateShippingRequestPriceOfferTransaction(string displayName, int value) : base(displayName, value)
        {
        }

        public override string Transaction { get => "PriceOfferCreatedTransaction" ; } // Without Spaces To Use It As Localized String Key
    }
    
    public class UpdateShippingRequestPriceOfferTransaction : EntityLogTransaction
    {
        public UpdateShippingRequestPriceOfferTransaction(string displayName, int value) : base(displayName, value)
        {
        }

        public override string Transaction { get => "PriceOfferUpdatedTransaction" ; } // Without Spaces To Use It As Localized String Key
    }
    
    public class AcceptShippingRequestPriceOfferTransaction : EntityLogTransaction
    {
        public AcceptShippingRequestPriceOfferTransaction(string displayName, int value) : base(displayName, value)
        {
        }

        public override string Transaction { get => "PriceOfferAcceptedTransaction" ; } // Without Spaces To Use It As Localized String Key
    }
    
}