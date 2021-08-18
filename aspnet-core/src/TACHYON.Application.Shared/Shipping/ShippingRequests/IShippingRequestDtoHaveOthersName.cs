namespace TACHYON.Shipping.ShippingRequests
{
    public interface IShippingRequestDtoHaveOthersName
    {
         int? TransportTypeId { get; set; }
         long TrucksTypeId { get; set; }
         int? GoodCategoryId { get; set; }
         string OtherGoodsCategoryName { get; set; }
         string OtherTransportTypeName { get; set; }
         string OtherTrucksTypeName { get; set; }
    }
}
