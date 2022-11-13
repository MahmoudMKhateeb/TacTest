using System.Collections.Generic;

namespace TACHYON.PriceOffers.Dto
{
    public class SrActorCarrierPriceForEditOutput
    {
        public ActorCarrierPriceDto ActorCarrierPrice { get; set; }
        
        public List<ActorCarrierPriceDto> VasActorCarrierPrices{ get; set; }

    }
}