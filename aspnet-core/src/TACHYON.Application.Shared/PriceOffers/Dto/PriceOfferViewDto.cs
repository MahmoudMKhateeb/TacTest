using Abp.Application.Services.Dto;
using System.Collections.Generic;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PriceOffers.Dto
{
    public class PriceOfferViewDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public int? EditionId { get; set; }
        public string Name { get; set; }
        public PriceOfferStatus Status { get; set; }
        public PriceOfferType PriceType { get; set; }
        public ShippingRequestStatus ShippingRequestStatus { get; set; }

        public bool IsTachyonDeal { get; set; }
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
        public string CommissionTypeTitle
        {
            get { return CommissionType.GetEnumDescription(); }
        }

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


    }

    public class GetOfferForViewOutput
    {
        public PriceOfferViewDto PriceOfferViewDto { get; set; }

        public bool CanIAcceptOffer { get; set; }
        public bool CanIAcceptOrRejectOfferOnBehalf { get; set; }
        public bool CanIEditOffer { get; set; }
    }



}
