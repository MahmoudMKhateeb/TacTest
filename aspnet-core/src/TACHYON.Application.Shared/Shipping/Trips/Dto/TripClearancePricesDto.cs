using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Commission;
using TACHYON.PriceOffers;

namespace TACHYON.Shipping.Trips.Dto
{
    public class TripClearancePricesDto : PriceCommissionDtoBase
    {
        public long ShippingRequestId { get; set; }
        public long RoutePointId { get; set; }
        public decimal ItemPrice {get; set;}
        public decimal TotalAmount {get; set;}
        public decimal SubTotalAmount {get; set;}
        public decimal VatAmount {get; set;}
        public decimal TotalAmountWithCommission {get; set;}
        public decimal SubTotalAmountWithCommission {get; set;}
        public decimal VatAmountWithCommission {get; set;}
        public decimal CommissionAmount {get; set;}
        public PriceOfferCommissionType CommissionType {get; set;}
        public decimal CommissionPercentageOrAddValue {get; set;}
    }
}
