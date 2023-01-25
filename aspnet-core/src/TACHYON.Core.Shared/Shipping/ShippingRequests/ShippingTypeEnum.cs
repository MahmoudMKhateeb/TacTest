using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests
{
    public enum ShippingTypeEnum
    {
        LocalInsideCity = 1,
        LocalBetweenCities = 2,
        [Description("Import port movement")]
        ImportPortMovements = 3,
        CrossBorderMovements = 4,
        [Description("export port movement")]
        ExportPortMovements = 5,

    }
}
