using Abp.Application.Services.Dto;

namespace TACHYON.PriceOffers.Dto
{
    
    public class ActorCarrierPriceDto : EntityDto
    {
        /// <summary>
        /// to pass Vas display name to front 
        /// </summary>
        public string VasDisplayName { get; set; }

        public long VasId { get; set; }

        public decimal? SubTotalAmount { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? TaxVat { get; set; }
    }
}