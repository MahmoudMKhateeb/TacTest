using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.MultiTenancy;
using TACHYON.PriceOffers.Base;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TrucksTypes;

namespace TACHYON.PricePackages
{
    public class BidNormalPricePackage : PriceOfferBase
    {
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
        public string PricePackageId { get; set; }
        [Required]
        [StringLength(PricePackagesConst.MaxDisplayNameLength, MinimumLength = PricePackagesConst.MinDisplayNameLength)]
        public string DisplayName { get; set; }
        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; }
        public int TransportTypeId { get; set; }
        [ForeignKey(nameof(TransportTypeId))]
        public TransportType TransportTypeFk { get; set; }
        public long TrucksTypeId { get; set; }
        [ForeignKey(nameof(TrucksTypeId))]
        public TrucksType TrucksTypeFk { get; set; }
        public List<BidPricePackageDetails> Items { get; set; }
        public int NormalPricePackageId { get; set; }
        [ForeignKey(nameof(NormalPricePackageId))]
        public NormalPricePackage NormalPricePackageFK { get; set; }

    }
}