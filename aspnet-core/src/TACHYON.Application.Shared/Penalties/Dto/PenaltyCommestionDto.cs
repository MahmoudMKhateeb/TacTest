using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.PriceOffers;

namespace TACHYON.Penalties.Dto
{
   public class PenaltyCommestionDto
    {
        public decimal CommissionValue { get; set; }
        public PriceOfferCommissionType CommissionType { get; set; }
        public decimal AmountPreCommestion { get; set; }
        public decimal AmountPostCommestion { get; set; }
        public decimal VatPreCommestion { get; set; }
        public decimal VatPostCommestion { get; set; }
        public decimal ItemPrice { get; set; }

        public decimal VatAmount()
        {
            return VatPostCommestion - VatPreCommestion;
        }
        public decimal TotalAmount()
        {
            return AmountPostCommestion + VatPostCommestion;
        }
    }
    
}
