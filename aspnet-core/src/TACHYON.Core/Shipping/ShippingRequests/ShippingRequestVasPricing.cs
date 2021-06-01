using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.ShippingRequestVases;
using TACHYON.Common;
namespace TACHYON.Shipping.ShippingRequests
{
    [Table("ShippingRequestVasesPricing")]
   public class ShippingRequestVasPricing:Entity<long>
    {
        public long ShippingRequestPricingId { get; set; }
        [ForeignKey(nameof(ShippingRequestPricingId))]
        public  ShippingRequestPricing ShippingRequestPricingFK { get; set; }

        public long VasId { get; set; }
        [ForeignKey(nameof(VasId))]
        public ShippingRequestVas ShippingRequestVasFK { get; set; }

        public decimal VasPrice { get; set; }
        public decimal VasVatAmount { get; set; }
        public decimal VasTotalAmount { get; set; }


        public decimal VasSubTotalAmountWithCommission { get; set; }
        public decimal VasVatAmountWithCommission { get; set; }
        public decimal VasTotalAmountWithCommission { get; set; }


        public decimal TotalAmount { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal VatAmount { get; set; }


        public decimal TotalAmountWithCommission { get; set; }
        public decimal SubTotalAmountWithCommission { get; set; }
        public decimal VatAmountWithCommission { get; set; }

        public decimal VasCommissionAmount { get; set; }

        public decimal CommissionAmount { get; set; }


        private decimal _taxVat;
        private ShippingRequestCommissionType _commissionType;
        private decimal _commissionPercentageOrAddValue;
        private int _totals;
        public void CalculateVas(decimal taxVat, ShippingRequestCommissionType commissionType,decimal commissionPercentageOrAddValue)
        {
            _taxVat = taxVat;
            _totals = ShippingRequestVasFK.RequestMaxAmount+ ShippingRequestVasFK.RequestMaxCount;
            _commissionType = commissionType;
            _commissionPercentageOrAddValue = commissionPercentageOrAddValue;
            CalculateCommission();
            CalculateSingleVas();
            CalculateMultipleVases();
            CalculateSingleVasWithCommission();
            CalculateMultipleVasesWithCommission();
        }
        private void CalculateSingleVas()
        {
            VasVatAmount = Calculate.CalculateVat(VasPrice, _taxVat);
            VasTotalAmount = VasPrice + VasVatAmount;

        }

        private void CalculateMultipleVases()
        {
            SubTotalAmount = VasPrice * _totals;
            TotalAmount = VasTotalAmount * _totals;
            VatAmount = VasVatAmount * _totals;
        }


        private void CalculateSingleVasWithCommission()
        {
            VasSubTotalAmountWithCommission = VasPrice + VasCommissionAmount;
            VasVatAmountWithCommission = TACHYON.Common.Calculate.CalculateVat(VasSubTotalAmountWithCommission, _taxVat);
            VasTotalAmountWithCommission = VasSubTotalAmountWithCommission + VasVatAmountWithCommission;

        }
        private void CalculateMultipleVasesWithCommission()
        {
            SubTotalAmountWithCommission = VasSubTotalAmountWithCommission * _totals;
            VatAmountWithCommission = VasVatAmountWithCommission * _totals;
            TotalAmountWithCommission = VasTotalAmountWithCommission * _totals;
            CommissionAmount = VasCommissionAmount * _totals;
        }
        private void CalculateCommission()
        {
            switch (_commissionType)
            {
                case ShippingRequestCommissionType.Percentage:
                    VasCommissionAmount = (VasPrice * _commissionPercentageOrAddValue / 100);
                    break;
                case ShippingRequestCommissionType.Value:
                    VasCommissionAmount = VasPrice + _commissionPercentageOrAddValue;
                    break;
            }
        }
    }
}
