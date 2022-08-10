namespace TACHYON.Shipping.Notes
{
    public enum VisibilityNotes : byte
    {
        Internal = 0,
        //Others
        TMSAndCarrier = 1,
        CarrierOnly = 2,
        TMSOnly = 3,
        ShipperOnly = 4
    }
}