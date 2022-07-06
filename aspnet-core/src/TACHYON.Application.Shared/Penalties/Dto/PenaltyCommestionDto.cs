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
        /// <summary>
        /// vat amount without commission
        /// </summary>
        public decimal VatAmount { get; set; }
        public decimal VatPostCommestion { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal TaxVat { get; set; }

        //public decimal VatAmount()
        //{
        //    return VatPostCommestion - VatPreCommestion;
        //   // return VatPostCommestion;
        //}
        public decimal TotalAmount()
        {
            return AmountPostCommestion + VatPostCommestion;
        }
    }
    
}
