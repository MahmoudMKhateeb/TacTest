using Abp.Application.Services.Dto;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class ShippingRequestCarrierPricingDto:EntityDto<long>
    {
        public decimal TripPrice { get; set; }
        public decimal TripVatAmount { get; set; }
        public decimal TripTotalAmount { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal VatAmount { get; set; }

        public decimal TaxVat { get; set; }

        public ShippingRequestPricingStatus Status { get; set; }

        public string StatusTitle { get { return Status.GetEnumDescription(); } }
    }
}
