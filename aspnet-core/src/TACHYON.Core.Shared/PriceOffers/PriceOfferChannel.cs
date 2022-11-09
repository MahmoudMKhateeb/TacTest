namespace TACHYON.PriceOffers
{
    public enum PriceOfferChannel : byte
    {
        MarketPlace=1,
        DirectRequest=2,
        TachyonManageService=3,
        // carrier as saas includes broker and carrier saas, any internal request called saas
        CarrierAsSaas=4,
        Offers=10
    }
}