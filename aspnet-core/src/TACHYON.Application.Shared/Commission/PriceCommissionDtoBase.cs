using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.PriceOffers;

namespace TACHYON.Commission
{
     public interface PriceCommissionDtoBase
     {
         decimal ItemPrice { get; set; }

         decimal TotalAmount { get; set; }
         decimal SubTotalAmount { get; set; }
         decimal VatAmount { get; set; }


         decimal TotalAmountWithCommission { get; set; }
         decimal SubTotalAmountWithCommission { get; set; }
         decimal VatAmountWithCommission { get; set; }
         decimal CommissionAmount { get; set; }
         PriceOfferCommissionType CommissionType { get; set; }
         decimal CommissionPercentageOrAddValue { get; set; }
     }
}
