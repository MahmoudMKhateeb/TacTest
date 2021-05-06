namespace TACHYON.Shipping.Trips
{
    public  enum ShippingRequestTripStatus:byte
    {
        New=0,
        Intransit = 1,
        Canceled = 2,
        Delivered = 3    
    }
}
