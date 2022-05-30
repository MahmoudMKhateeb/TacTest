using System.ComponentModel;

namespace TACHYON.Shipping.ShippingRequests
{
    public enum ShippingRequestDirectRequestStatus : byte
    {
        New = 0,
        Response = 1,
        Accepted = 2,
        Rejected = 3,
        [Description("DeclinedOfPricing")] Declined = 5,
        Pending = 6,
        Closed = 7
    }
}