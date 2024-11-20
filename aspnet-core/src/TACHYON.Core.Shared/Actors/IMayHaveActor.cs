using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TACHYON.Actors
{
    public interface IMayHaveShipperActor
    {
        int? ShipperActorId { get; set; }

    }

    public interface IMayHaveCarrierActor
    {
        int? CarrierActorId { get; set; }

    }

}
