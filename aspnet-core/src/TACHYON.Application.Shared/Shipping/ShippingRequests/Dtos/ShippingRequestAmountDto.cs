using Abp.Application.Services.Dto;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class ShippingRequestAmountDto:EntityDto
    {
        public long ShippingRequestId { get; set; }
        public int? CarrirerTenantId { get; set; }
        public int? DirectRequestId { get; set; }
        public int? shippingRequestBidId { get; set; }
        public string ClientName { get; set; }
        public decimal CarrierPrice { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalCommission { get; set; }
        public decimal VatSetting { get; set; }
        public decimal ActualCommissionValue { get; set; }
        public decimal ActualPercentCommission { get; set; }
        public decimal MinCommissionValueSetting { get; set; }
    }
}
