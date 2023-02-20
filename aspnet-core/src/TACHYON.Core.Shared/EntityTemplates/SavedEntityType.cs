using System.ComponentModel;

namespace TACHYON.EntityTemplates
{
    public enum SavedEntityType
    {
        [Description("ShippingRequestSavedTemplate")]
        ShippingRequestTemplate = 1,
        [Description("TripSavedTemplate")]
        TripTemplate = 2,
        [Description("DedicatedShippingRequestSavedTemplate")]
        DedicatedShippingRequestTemplate = 3,
    }
}