namespace TACHYON.PriceOffers.Dto
{
    /// <summary>
    /// General item like vas or trip or dedicated 
    /// </summary>
    public class PriceOfferItemDto
    {
        public long ParentItemId { get; set; }
        public long ItemId { get; set; }
        public string ItemName { get; set; }
        public PriceOfferType PriceType { get; set; } 
        public decimal? Price { get; set; }
        public int Quantity { get; set; }
        public int Amount { get; set; }

    }
}
