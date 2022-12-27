using Abp.Application.Services.Dto;

namespace TACHYON.PriceOffers.Dto
{
    public class ActorShipperPriceDto: EntityDto
    {
        /// <summary>
        /// to pass Vas display name to front 
        /// </summary> 
        public string VasDisplayName { get; set; }

        public long VasId{ get; set; }


        public decimal? TotalAmountWithCommission { get; set; }
        public decimal? SubTotalAmountWithCommission { get; set; }
        public decimal? VatAmountWithCommission { get; set; }
        public decimal? TaxVat { get; set; }
    }
}