using Abp.Application.Features;
using Abp.Configuration;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Configuration;
using TACHYON.Features;
using TACHYON.MultiTenancy;

namespace TACHYON.Shipping.ShippingRequests
{

    [Table("ShippingRequestPricings")]
    public class ShippingRequestPricing : FullAuditedEntity<long>, IMustHaveTenant
    {
        public long? ParentId { get; set; }
        [ForeignKey(nameof(ParentId))]
        public ShippingRequestPricing ShippingRequestPricingFK { get; set; }
        public long ShippingRequestId { get; set; }

        [ForeignKey(nameof(ShippingRequest))]
        public ShippingRequest ShippingRequestFK { get; set; }
        /// <summary>
        /// Get id from source entity
        /// </summary>
        public long SourceId { get; set; }

        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; }

        public ShippingRequestPricingChannel Channel { get; set; }
        public ShippingRequestPricingStatus Status { get; set; }
        #region Invoice
        #region Single trip pricing for carrier
        public decimal TripPrice { get; set; }
        public decimal TripVatAmount { get; set; }
        public decimal TripTotalAmount { get; set; }

        #endregion
        #region Single trip pricing with commission for shipper or tachyon dealer
        public decimal TripSubTotalAmountWithCommission { get; set; }
        public decimal TripVatAmountWithCommission { get; set; }
        public decimal TripTotalAmountWithCommission { get; set; }
        #endregion
        #region Pricing Totals of trips and vass
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
        public ShippingRequestCommissionType CommissionType { get; set; }
        public decimal TripCommissionAmount { get; set; }
        public decimal CommissionValue { get; set; }
        public decimal CommissionPercentage { get; set; }
        public decimal CommissionAmount { get; set; }
        #endregion


        /// <summary>
        /// If shipper reject offer, will place reason of rejected
        /// </summary>
        public string RejectedReason { get; set; }
        /// <summary>
        /// If the shipper or TAD view this pricing,Help us when the carrier edit the price to check if the is view sent notification to stachholder the the carrier update price else sent there new price add.
        /// </summary>
        public bool IsView { get; set; }
        private IFeatureChecker _featureChecker;
        private ShippingRequest _shippingRequest;
        public void Calculate(IFeatureChecker featureChecker, ISettingManager settingManager, ShippingRequest shippingRequest)
        {
            _featureChecker = featureChecker;
            _shippingRequest = shippingRequest;
            TaxVat = settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);
            SetCommissionSettings();
            CalculateSingleTrip();
            CalculateMultipleTrip();
            CalculateSingleTripWithCommission();
            CalculateMultipleTripWithCommission();

        }
        private void CalculateSingleTrip()
        {
            TripVatAmount = CalculateVat(TripPrice, TaxVat);
            TotalAmount = TripPrice + TripVatAmount;
          
        }

        private void CalculateMultipleTrip()
        {
            SubTotalAmount = TripPrice * _shippingRequest.NumberOfTrips;
            TotalAmount = TripTotalAmount * _shippingRequest.NumberOfTrips;
            VatAmount = TripVatAmount * _shippingRequest.NumberOfTrips;
        }
        private void CalculateSingleTripWithCommission()
        {
            TripSubTotalAmountWithCommission = TripPrice + TripCommissionAmount;
            TripVatAmountWithCommission = CalculateVat(TripSubTotalAmountWithCommission, TaxVat);
            TripTotalAmountWithCommission = TripSubTotalAmountWithCommission + TripVatAmountWithCommission;
          
        }
        private void CalculateMultipleTripWithCommission()
        {
            SubTotalAmountWithCommission = TripSubTotalAmountWithCommission * _shippingRequest.NumberOfTrips;
            VatAmountWithCommission = TripVatAmountWithCommission * _shippingRequest.NumberOfTrips;
            TotalAmountWithCommission = TripTotalAmountWithCommission * _shippingRequest.NumberOfTrips;
        }
        private decimal CalculateVat(decimal amount, decimal vat)
        {
            return (amount / 100) * vat;
        }

        private void SetCommissionSettings()
        {
            if (_shippingRequest.IsTachyonDeal)
            {
                CommissionType = (ShippingRequestCommissionType)Convert.ToByte(_featureChecker.GetValue(_shippingRequest.TenantId, AppFeatures.TachyonDealerCommissionType));
                switch (CommissionType)
                {
                    case ShippingRequestCommissionType.Percentage:
                        CommissionPercentage = Convert.ToDecimal(_featureChecker.GetValue(_shippingRequest.TenantId, AppFeatures.TachyonDealerCommissionPercentage));
                        CommissionValue = 0;
                        break;
                    case ShippingRequestCommissionType.Value:
                        CommissionValue = Convert.ToDecimal(_featureChecker.GetValue(_shippingRequest.TenantId, AppFeatures.TachyonDealerCommissionValue));
                        CommissionPercentage = 0;
                        break;
                }
            }
            else
            {
                CommissionType = (ShippingRequestCommissionType)Convert.ToByte(_featureChecker.GetValue(_shippingRequest.TenantId, AppFeatures.CommissionType));
                switch (CommissionType)
                {
                    case ShippingRequestCommissionType.Percentage:
                        CommissionPercentage = Convert.ToDecimal(_featureChecker.GetValue(_shippingRequest.TenantId, AppFeatures.BiddingCommissionPercentage));
                        CommissionValue = 0;
                        break;
                    case ShippingRequestCommissionType.Value:
                        CommissionPercentage = 0;
                        CommissionValue = Convert.ToDecimal(_featureChecker.GetValue(_shippingRequest.TenantId, AppFeatures.BiddingCommissionValue));
                        break;
                }
            }

            CalculateTripCommission();
        }
        private void CalculateTripCommission()
        {
            switch (CommissionType)
            {
                case ShippingRequestCommissionType.Percentage:
                    TripCommissionAmount = (TripPrice * CommissionPercentage / 100);
                    break;
                case ShippingRequestCommissionType.Value:
                    TripCommissionAmount = TripPrice + CommissionValue;
                    break;
            }
        }
    }
}
