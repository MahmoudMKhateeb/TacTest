using System.ComponentModel.DataAnnotations;

namespace TACHYON.TachyonPriceOffers.dtos
{
    public class CreateOrEditTachyonPriceOfferDto
    {
        public int? CarrirerTenantId { get; set; }
        public long? ShippingRequestBidId { get; set; }
        public int? DriectRequestForCarrierId { get; set; }
        [Required]
        public virtual long ShippingRequestId { get; set; }
        public decimal? CarrierPrice { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        public decimal ActualCommissionValue { get; set; }
        public decimal ActualPercentCommission { get; set; }



    }
}
