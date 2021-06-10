namespace TACHYON.PriceOffers
{
    public  enum PriceOfferStatus:byte
    {
        New = 0,
        Accepted = 1, //accepted and there is assigned carrier
        Rejected = 2,
        AcceptedAndWaitingForCarrier = 3
    }
}
