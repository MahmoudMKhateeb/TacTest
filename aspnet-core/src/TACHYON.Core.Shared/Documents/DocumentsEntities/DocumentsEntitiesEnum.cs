using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TACHYON.Documents.DocumentsEntities
{
    [DataContract]
    public enum DocumentsEntitiesEnum : int
    {
        [Description("Tenant")] Tenant = 1,
        [Description("Driver")] Driver = 2,
        [Description("Truck")] Truck = 3,
        [Description("Trip")] Trip = 4,
        [Description("ActorShipper")] 
        ActorShipper = 5,
        [Description("ActorCarrier")] ActorCarrier = 6,
    }
}