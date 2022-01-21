namespace TACHYON.Offers.Dtos
{
    public class GetOfferForViewDto
    {
        public OfferDto Offer { get; set; }

        public string TrucksTypeDisplayName { get; set; }

        public string TrailerTypeDisplayName { get; set; }

        public string GoodCategoryDisplayName { get; set; }

        public string RouteDisplayName { get; set; }
    }
}