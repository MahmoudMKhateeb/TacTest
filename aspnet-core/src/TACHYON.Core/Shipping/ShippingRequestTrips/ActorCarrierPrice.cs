using Abp.Auditing;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.PriceOffers;
using TACHYON.ShippingRequestTripVases;

namespace TACHYON.Shipping.ShippingRequestTrips
{
    [Table("ActorCarrierPrice")]
    public class ActorCarrierPrice : FullAuditedEntity
    {
        public int? ShippingRequestTripId { get; set; }

        [ForeignKey("ShippingRequestTripId")]
        public ShippingRequestTrip ShippingRequestTripFk { get; set; }


        public long? ShippingRequestTripVasId { get; set; }
        [ForeignKey("ShippingRequestTripVasId")]
        public ShippingRequestTripVas ShippingRequestTripVasFk { get; set; }



        public bool IsActorCarrierHaveInvoice { get; set; }
        public decimal? SubTotalAmount { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? TaxVat { get; set; }

    }
}
