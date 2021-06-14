using Abp.Application.Services.Dto;
using TACHYON.PriceOffers;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class ShippingRequestCarrierPricingDto:EntityDto<long>
    {
        public decimal ItemPrice { get; set; }
        public decimal ItemVatAmount { get; set; }
        public decimal ItemTotalAmount { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal VatAmount { get; set; }

        public decimal TaxVat { get; set; }

        public PriceOfferStatus Status { get; set; }

        public string StatusTitle { get { return Status.GetEnumDescription(); } }
    }
}
