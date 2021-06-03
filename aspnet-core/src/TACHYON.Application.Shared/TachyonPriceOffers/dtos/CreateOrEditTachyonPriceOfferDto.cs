using Abp.Application.Services.Dto;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.TachyonPriceOffers.dtos
{
    public class CreateOrEditTachyonPriceOfferDto :EntityDto<int?>
    {
        [JsonIgnore]
        public int? CarrirerTenantId { get; set; }
        [JsonIgnore]
        public decimal? CarrierPrice { get; set; }
        public long? ShippingRequestBidId { get; set; }
        public int? DriectRequestForCarrierId { get; set; }
        public virtual long? ShippingRequestId { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }
        public decimal ActualCommissionValue { get; set; }
        public decimal ActualPercentCommission { get; set; }

    }
}
