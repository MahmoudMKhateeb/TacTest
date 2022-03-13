using System.ComponentModel;

namespace TACHYON.EntityTemplates
{
    public enum SavedEntityType
    {
        [Description("ShippingRequestSavedTepmlate")]
        ShippingRequestTemplate = 1,
        [Description("TripSavedTepmlate")]
        TripTemplate = 2
    }
}