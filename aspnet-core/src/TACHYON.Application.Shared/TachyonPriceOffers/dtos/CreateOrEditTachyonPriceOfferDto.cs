using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.TachyonPriceOffers.dtos
{
    public class CreateOrEditTachyonPriceOfferDto
    {
        [JsonIgnore]
        public int? CarrirerTenantId { get; set; }
        public long? ShippingRequestBidId { get; set; }
        public int? DriectRequestForCarrierId { get; set; }
        [Required]
        public virtual long ShippingRequestId { get; set; }
        [JsonIgnore]
        public decimal? CarrierPrice { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        public decimal ActualCommissionValue { get; set; }
        public decimal ActualPercentCommission { get; set; }



    }
}
