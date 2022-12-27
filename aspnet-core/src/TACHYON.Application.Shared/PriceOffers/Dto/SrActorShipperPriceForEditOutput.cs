using System.Collections.Generic;

namespace TACHYON.PriceOffers.Dto
{
    public class SrActorShipperPriceForEditOutput
    {
        public ActorShipperPriceDto ActorShipperPrice { get; set; }

        public List<ActorShipperPriceDto> VasActorShipperPrices { get; set; }
    }
}