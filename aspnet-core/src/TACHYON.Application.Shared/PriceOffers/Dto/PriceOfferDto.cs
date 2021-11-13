using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace TACHYON.PriceOffers.Dto
{
    public class PriceOfferDto : EntityDto<long>
    {
        public PriceOfferStatus Status { get; set; }
        public PriceOfferType PriceType { get; set; }

        public long? ParentId { get; set; }
        #region Invoice
        #region Single  pricing for carrier
        public decimal ItemPrice { get; set; }
        public decimal ItemVatAmount { get; set; }
        public decimal ItemTotalAmount { get; set; }

        #endregion
        #region Single item  pricing with commission for shipper or tachyon dealer
        public decimal ItemSubTotalAmountWithCommission { get; set; }
        public decimal ItemVatAmountWithCommission { get; set; }
        public decimal ItemTotalAmountWithCommission { get; set; }
        #endregion
        #region Pricing Totals of Items and Details
        public decimal ItemsTotalPricePreCommissionPreVat { get; set; }

        public decimal ItemsTotalVatAmountPreCommission { get; set; }
        public decimal ItemsTotalCommission { get; set; }
        public decimal ItemsTotalPricePostCommissionPreVat { get; set; }
        public decimal ItemsTotalVatPostCommission { get; set; }

        public decimal DetailsTotalPricePreCommissionPreVat { get; set; }

        public decimal DetailsTotalVatAmountPreCommission { get; set; }
        public decimal DetailsTotalCommission { get; set; }

        public decimal DetailsTotalPricePostCommissionPreVat { get; set; }

        public decimal DetailsTotalVatPostCommission { get; set; }


        public decimal TotalAmount { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal VatAmount { get; set; }


        public decimal TotalAmountWithCommission { get; set; }
        public decimal SubTotalAmountWithCommission { get; set; }
        public decimal VatAmountWithCommission { get; set; }
        #endregion
        public decimal TaxVat { get; set; }
        #endregion
        #region Commission
        public PriceOfferCommissionType CommissionType { get; set; }

        public decimal ItemCommissionAmount { get; set; }
        public decimal CommissionPercentageOrAddValue { get; set; }
        public decimal CommissionAmount { get; set; }

        public PriceOfferCommissionType VasCommissionType { get; set; }
        public decimal VasCommissionPercentageOrAddValue { get; set; }

        #endregion

        public int Quantity { get; set; } = 1;
        public ICollection<PriceOfferItem> Items { get; set; } = new List<PriceOfferItem>();
        /// <summary>
        /// If shipper reject offer, will place reason of rejected
        /// </summary>
        public string RejectedReason { get; set; }

        public PriceOfferTenantCommissionSettings CommissionSettings { get; set; }
    }
    public class PriceOfferTenantCommissionSettings
    {
        public PriceOfferCommissionType ItemCommissionType { get; set; }
        public decimal ItemCommissionPercentage { get; set; }
        public decimal ItemCommissionValue { get; set; }
        public decimal ItemMinValueCommission { get; set; }

        public PriceOfferCommissionType VasCommissionType { get; set; }

        public decimal VasCommissionPercentage { get; set; }
        public decimal VasCommissionValue { get; set; }
        public decimal VasMinValueCommission { get; set; }
    }

}