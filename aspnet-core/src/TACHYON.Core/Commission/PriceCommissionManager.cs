using Abp.Application.Features;
using Abp.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Configuration;
using TACHYON.Features;
using TACHYON.Offers;
using TACHYON.PriceOffers;

namespace TACHYON.Commission
{
    public class PriceCommissionManager: TACHYONDomainServiceBase
    {
        private decimal TaxVat;
        private readonly ISettingManager _settingManager;
        public PriceCommissionManager(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }
        public void Calculate(PriceCommissionDtoBase priceCommissionDto)
        {
            TaxVat =_settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);
            // ItemCommissionAmount 
            CalculateCommission(priceCommissionDto);
            // ItemVatAmount, ItemTotalAmount
            CalculatePreCommission(priceCommissionDto);
            // ItemSubTotalAmountWithCommission, ItemVatAmountWithCommission, ItemTotalAmountWithCommission
            CalculateWithCommission(priceCommissionDto);
        }


        private void CalculateCommission(PriceCommissionDtoBase item)
        {
            switch (item.CommissionType)
            {
                case PriceOfferCommissionType.CommissionPercentage:
                    item.CommissionAmount = (item.ItemPrice * item.CommissionPercentageOrAddValue / 100);

                    break;
                case PriceOfferCommissionType.CommissionValue:
                case PriceOfferCommissionType.CommissionMinimumValue:
                    item.CommissionAmount = item.CommissionPercentageOrAddValue;
                    break;
            }
        }

        private void CalculatePreCommission(PriceCommissionDtoBase item)
        {
            item.VatAmount = TACHYON.Common.Calculate.CalculateVat(item.ItemPrice, TaxVat);
            item.TotalAmount = item.ItemPrice + item.VatAmount;
            item.SubTotalAmount = item.ItemPrice;
        }


        private void CalculateWithCommission(PriceCommissionDtoBase item)
        {
            item.SubTotalAmountWithCommission = item.ItemPrice + item.CommissionAmount;
            item.VatAmountWithCommission =
                TACHYON.Common.Calculate.CalculateVat(item.SubTotalAmountWithCommission, TaxVat);
            item.TotalAmountWithCommission =
                item.SubTotalAmountWithCommission + item.VatAmountWithCommission;
        }

    }
}
