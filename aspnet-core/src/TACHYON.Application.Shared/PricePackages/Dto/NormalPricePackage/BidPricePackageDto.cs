using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.PriceOffers;

namespace TACHYON.PricePackages.Dto.NormalPricePackage
{
    public class BidNormalPricePackageDto
    {
        public string DisplayName { get; set; }
        public string PricePackageId { get; set; }
        public string TruckType { get; set; }
        public int OriginCityId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public int DestinationCityId { get; set; }
        public int TransportTypeId { get; set; }
        public long TrucksTypeId { get; set; }
        public bool IsMultiDrop { get; set; }
        public int NumberOfDrops { get; set; }
        public int NumberOfTrips { get; set; }
        public decimal SingleDropPrice { get; set; }
        public decimal? PricePerExtraDrop { get; set; }
        public long NormalPricePackageId { get; set; }
        public int TenantId { get; set; }
        public List<BidNormalPricePackageItemDto> Items { get; set; }
        /// <summary>
        /// Get id from source entity
        /// </summary>
        public long? SourceId { get; set; }

        public PriceOfferType PriceType { get; set; }
        #region Invoice


        #region Single  pricing for carrier
        public decimal ItemPrice { get; set; }
        /// <summary>
        /// ItemPrice * TaxVat
        /// </summary>
        public decimal ItemVatAmount { get; set; }
        /// <summary>
        ///  ItemPrice + ItemVatAmount;
        /// </summary>
        public decimal ItemTotalAmount { get; set; }



        #endregion


        #region Single item  pricing with commission for shipper or tachyon dealer
        /// <summary>
        /// ItemPrice + ItemCommissionAmount
        /// </summary>
        public decimal ItemSubTotalAmountWithCommission { get; set; }

        /// <summary>
        /// ItemSubTotalAmountWithCommission * TaxVat
        /// </summary>
        public decimal ItemVatAmountWithCommission { get; set; }

        /// <summary>
        ///  ItemSubTotalAmountWithCommission + ItemVatAmountWithCommission;
        /// (ItemPrice + ItemCommissionAmount) + ((ItemPrice + ItemCommissionAmount) * TaxVat)
        /// </summary>
        public decimal ItemTotalAmountWithCommission { get; set; }
        #endregion


        #region Pricing Totals of Items and Details
        /// <summary>
        /// Price Per trip * Number of Trips.
        /// </summary>
        public decimal ItemsTotalPricePreCommissionPreVat { get; set; }


        /// <summary>
        /// ( ItemTotalAmount * Quantity ) + PriceOfferDetails.Sum(x => x.TotalAmount)
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// (ItemPrice *  Quantity) + PriceOfferDetails.Sum(x => x.SubTotalAmount)
        /// </summary>
        public decimal SubTotalAmount { get; set; }


        /// <summary>
        /// (ItemVatAmount  Quantity) + PriceOfferDetails.Sum(x => x.VatAmount)
        /// </summary>
        public decimal VatAmount { get; set; }

        /// <summary>
        /// (ItemTotalAmountWithCommission *  Quantity) + PriceOfferDetails.Sum(x => x.TotalAmountWithCommission)
        /// </summary>
        public decimal TotalAmountWithCommission { get; set; }

        /// <summary>
        /// (ItemSubTotalAmountWithCommission *  Quantity) + PriceOfferDetails.Sum(x => x.SubTotalAmountWithCommission)
        /// </summary>
        public decimal SubTotalAmountWithCommission { get; set; }

        /// <summary>
        /// (ItemVatAmountWithCommission *  Quantity) + PriceOfferDetails.Sum(x => x.VatAmountWithCommission)
        /// </summary>
        public decimal VatAmountWithCommission { get; set; }
        #endregion

        #endregion

        #region Commission
        /// <summary>
        /// set of predefined settings for the Commission that is applied to Shippers according to their subscriptions type.
        /// </summary>
        public PriceOfferCommissionType CommissionType { get; set; }
        public decimal ItemCommissionAmount { get; set; }
        public decimal CommissionPercentageOrAddValue { get; set; }

        /// <summary>
        /// ( ItemCommissionAmount *  Quantity) + PriceOfferDetails.Sum(x => x.CommissionAmount)
        /// </summary>
        public decimal CommissionAmount { get; set; }
        #endregion

        /// <summary>
        /// shippingRequest.NumberOfTrips
        /// </summary>
        public int Quantity { get; set; } = 1;
        #region Invoice

        #region Pricing Totals of Items and Details
        /// <summary>
        /// ItemsTotalPricePreCommissionPreVat * TaxVat
        /// </summary>
        public decimal ItemsTotalVatAmountPreCommission { get; set; }

        /// <summary>
        /// ItemCommissionAmount *  Quantity
        /// </summary>
        public decimal ItemsTotalCommission { get; set; }

        /// <summary>
        /// ItemsTotalCommission + ItemsTotalPricePreCommissionPreVat
        /// </summary>
        public decimal ItemsTotalPricePostCommissionPreVat { get; set; }

        /// <summary>
        /// ItemsTotalPricePostCommissionPreVat + ItemsTotalCommission
        /// </summary>
        public decimal ItemsTotalVatPostCommission { get; set; }

        /// <summary>
        /// Summation of a For All VAS (Price Per VAS * Number of VAS Items).
        /// </summary>
        public decimal DetailsTotalPricePreCommissionPreVat { get; set; }

        /// <summary>
        /// DetailsTotalPricePreCommissionPreVat * TaxVat
        /// </summary>
        public decimal DetailsTotalVatAmountPreCommission { get; set; }

        /// <summary>
        /// PriceOfferDetails.Sum(x => x.CommissionAmount)
        /// </summary>
        public decimal DetailsTotalCommission { get; set; }

        /// <summary>
        /// DetailsTotalCommission + DetailsTotalPricePreCommissionPreVat
        /// </summary>
        public decimal DetailsTotalPricePostCommissionPreVat { get; set; }

        /// <summary>
        /// DetailsTotalPricePostCommissionPreVat + DetailsTotalCommission
        /// </summary>
        public decimal DetailsTotalVatPostCommission { get; set; }


        #endregion

        /// <summary>
        /// AppSettings.HostManagement.TaxVat
        /// </summary>
        public decimal TaxVat { get; set; }
        #endregion
    }
    public class BidNormalPricePackageItemDto
    {
        public string ItemName { get; set; }


        /// <summary>
        /// Get id from source entity
        /// </summary>
        public long? SourceId { get; set; }

        public PriceOfferType PriceType { get; set; }
        #region Invoice


        #region Single  pricing for carrier
        public decimal ItemPrice { get; set; }
        /// <summary>
        /// ItemPrice * TaxVat
        /// </summary>
        public decimal ItemVatAmount { get; set; }
        /// <summary>
        ///  ItemPrice + ItemVatAmount;
        /// </summary>
        public decimal ItemTotalAmount { get; set; }



        #endregion


        #region Single item  pricing with commission for shipper or tachyon dealer
        /// <summary>
        /// ItemPrice + ItemCommissionAmount
        /// </summary>
        public decimal ItemSubTotalAmountWithCommission { get; set; }

        /// <summary>
        /// ItemSubTotalAmountWithCommission * TaxVat
        /// </summary>
        public decimal ItemVatAmountWithCommission { get; set; }

        /// <summary>
        ///  ItemSubTotalAmountWithCommission + ItemVatAmountWithCommission;
        /// (ItemPrice + ItemCommissionAmount) + ((ItemPrice + ItemCommissionAmount) * TaxVat)
        /// </summary>
        public decimal ItemTotalAmountWithCommission { get; set; }
        #endregion


        #region Pricing Totals of Items and Details
        /// <summary>
        /// Price Per trip * Number of Trips.
        /// </summary>
        public decimal ItemsTotalPricePreCommissionPreVat { get; set; }


        /// <summary>
        /// ( ItemTotalAmount * Quantity ) + PriceOfferDetails.Sum(x => x.TotalAmount)
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// (ItemPrice *  Quantity) + PriceOfferDetails.Sum(x => x.SubTotalAmount)
        /// </summary>
        public decimal SubTotalAmount { get; set; }


        /// <summary>
        /// (ItemVatAmount  Quantity) + PriceOfferDetails.Sum(x => x.VatAmount)
        /// </summary>
        public decimal VatAmount { get; set; }

        /// <summary>
        /// (ItemTotalAmountWithCommission *  Quantity) + PriceOfferDetails.Sum(x => x.TotalAmountWithCommission)
        /// </summary>
        public decimal TotalAmountWithCommission { get; set; }

        /// <summary>
        /// (ItemSubTotalAmountWithCommission *  Quantity) + PriceOfferDetails.Sum(x => x.SubTotalAmountWithCommission)
        /// </summary>
        public decimal SubTotalAmountWithCommission { get; set; }

        /// <summary>
        /// (ItemVatAmountWithCommission *  Quantity) + PriceOfferDetails.Sum(x => x.VatAmountWithCommission)
        /// </summary>
        public decimal VatAmountWithCommission { get; set; }
        #endregion

        #endregion

        #region Commission
        /// <summary>
        /// set of predefined settings for the Commission that is applied to Shippers according to their subscriptions type.
        /// </summary>
        public PriceOfferCommissionType CommissionType { get; set; }
        public decimal ItemCommissionAmount { get; set; }
        public decimal CommissionPercentageOrAddValue { get; set; }

        /// <summary>
        /// ( ItemCommissionAmount *  Quantity) + PriceOfferDetails.Sum(x => x.CommissionAmount)
        /// </summary>
        public decimal CommissionAmount { get; set; }
        #endregion

        /// <summary>
        /// shippingRequest.NumberOfTrips
        /// </summary>
        public int Quantity { get; set; } = 1;

    }
}