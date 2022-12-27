using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;
using TACHYON.Common;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Tracking
{
    public class PointTransactionArgs
    {
        public long PointId { get; set; }
        public string Code { get; set; }
        public List<IHasDocument> Documents { get; set; }
        
        [JsonIgnore] 
        [DefaultValue(false)]
        public bool ForceDeliverModeEnabled { get; set; }
    }
}