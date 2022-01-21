namespace TACHYON.Shipping.Trips
{
    public enum ShippingRequestTripStatus : byte
    {
        New = 0,
        Intransit = 1,
        Canceled = 2,
        Delivered = 3,

        /// <summary>
        /// When one or more drop in that trip needs receiver confirmation or POD or both
        /// </summary>
        DeliveredAndNeedsConfirmation = 4
    }
}