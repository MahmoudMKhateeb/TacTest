using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests
{
    public enum ShippingTypeEnum
    {
        LocalInsideCity = 1,
        LocalBetweenCities = 2,
        ImportPortMovements = 3,
        CrossBorderMovements = 4,
        ExportPortMovements = 5,

    }
}
