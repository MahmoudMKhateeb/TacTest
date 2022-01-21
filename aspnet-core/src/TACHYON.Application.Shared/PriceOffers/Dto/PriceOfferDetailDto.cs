using Newtonsoft.Json;

namespace TACHYON.PriceOffers.Dto
{
    public class PriceOfferDetailDto
    {
        public long ItemId { get; set; }
        public int Price { get; set; }
        [JsonIgnore] public decimal? CommissionPercentageOrAddValue { get; set; }
        [JsonIgnore] public PriceOfferType PriceType { get; set; } = PriceOfferType.Vas;
        [JsonIgnore] public PriceOfferCommissionType CommissionType { get; set; }
    }
}