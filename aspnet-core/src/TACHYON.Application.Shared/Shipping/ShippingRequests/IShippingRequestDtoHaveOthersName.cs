namespace TACHYON.Shipping.ShippingRequests
{
    public interface IShippingRequestDtoHaveOthersName
    {
        int? TransportTypeId { get; set; }
        long TrucksTypeId { get; set; }
        int? GoodCategoryId { get; set; }
        int PackingTypeId { get; set; }
        string OtherGoodsCategoryName { get; set; }
        string OtherTransportTypeName { get; set; }
        string OtherTrucksTypeName { get; set; }
        string OtherPackingTypeName { get; set; }
    }
}