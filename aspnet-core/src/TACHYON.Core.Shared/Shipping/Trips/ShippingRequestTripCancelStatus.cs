
namespace TACHYON.Shipping.Trips
{
    public enum ShippingRequestTripCancelStatus :byte
    {
        None=0,
        Canceled=1,
        WaitingForTMSApproval=2,
        Rejected=3
    }
}
