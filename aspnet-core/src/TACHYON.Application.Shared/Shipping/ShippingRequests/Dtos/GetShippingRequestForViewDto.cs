namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class GetShippingRequestForViewDto
    {
		public ShippingRequestDto ShippingRequest { get; set; }

		public string TrucksTypeDisplayName { get; set;}

		public string TrailerTypeDisplayName { get; set;}

		public string GoodsDetailName { get; set;}

		public string RouteDisplayName { get; set;}


    }
}