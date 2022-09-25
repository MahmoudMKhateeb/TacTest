using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Actors;

namespace TACHYON.Shipping.ShippingRequests
{
    public interface IMayHaveShipperActor
    {
        public int? ShipperActorId { get; set; }

    } 
    
    public interface IMayHaveCarrierActor
    {
        public int? CarrierActorId { get; set; }

    }

}
