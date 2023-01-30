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
        [Description("port movements - Import")]
        ImportPortMovements = 3,
        CrossBorderMovements = 4,
        [Description("port movements - export")]
        ExportPortMovements = 5,

    }
}
