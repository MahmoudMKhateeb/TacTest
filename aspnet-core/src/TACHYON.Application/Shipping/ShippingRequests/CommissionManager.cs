using Abp.Application.Features;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using System;
using System.Threading.Tasks;
using TACHYON.Configuration;
using TACHYON.Features;
using TACHYON.Shipping.ShippingRequestBids;
using TACHYON.TachyonPriceOffers;

namespace TACHYON.Shipping.ShippingRequests
{
    public class CommissionManager : TACHYONDomainServiceBase
    {
        private readonly IFeatureChecker _featureChecker;
        private readonly ISettingManager _settingManager;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;

        public CommissionManager(IFeatureChecker featureChecker, ISettingManager settingManager, IRepository<ShippingRequest, long> shippingRequestRepository)
        {
            _featureChecker = featureChecker;
            _settingManager = settingManager;
            _shippingRequestRepository = shippingRequestRepository;
        }
        
        public void AddShippingRequestCommissionSettingInfo(ShippingRequest shippingRequest)
        {
            if (shippingRequest.IsBid)
            {
                shippingRequest.CommissionValueSetting =
                    Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId,
                        AppFeatures.BiddingCommissionValue));
                shippingRequest.PercentCommissionSetting = Convert.ToDecimal(
                    _featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.BiddingCommissionPercentage));
                shippingRequest.MinValueCommissionSetting = Convert.ToDecimal(
                    _featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.BiddingMinValueCommission));
            }
            else if (shippingRequest.IsTachyonDeal)
            {
                shippingRequest.CommissionValueSetting =
                    Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId,
                        AppFeatures.TachyonDealerCommissionValue));
                shippingRequest.PercentCommissionSetting = Convert.ToDecimal(
                    _featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerCommissionPercentage));
                shippingRequest.MinValueCommissionSetting = Convert.ToDecimal(
                    _featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerMinValueCommission));
            }
            shippingRequest.VatSetting= _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);
        }

        public async Task AddCommissionInfoAfterCarrierBid(ShippingRequestBid shippingRequestBid)
        {
            var request =await GetShippingRequest(shippingRequestBid.ShippingRequestId);
            if (!request.IsTachyonDeal)
            {
                var basePrice = shippingRequestBid.BasePrice;
                var commissionValueSetting = request.CommissionValueSetting;
                var commissionPercentSetting = request.PercentCommissionSetting;
                var minShippmentSetting = request.MinValueCommissionSetting;

                var totalCommission = (basePrice * commissionPercentSetting / 100) + commissionValueSetting;

                if (totalCommission < minShippmentSetting)
                {
                    shippingRequestBid.ActualCommissionValue = 0;
                    shippingRequestBid.ActualPercentCommission = 0;
                    shippingRequestBid.ActualMinCommissionValue = minShippmentSetting;
                    totalCommission = minShippmentSetting;
                }
                else
                {
                    shippingRequestBid.ActualCommissionValue = commissionValueSetting;
                    shippingRequestBid.ActualPercentCommission = commissionPercentSetting;
                }

                shippingRequestBid.TotalCommission = (decimal)totalCommission;
                //shippingRequestBid.price = (decimal)(basePrice + totalCommission + request.VatSetting);
                var amount = basePrice + totalCommission;
                shippingRequestBid.PriceSubTotal = amount;
                shippingRequestBid.VatAmount = CalculateVat(amount, request.VatSetting);
                shippingRequestBid.price= CalculatePriceIncludeVat(amount, request.VatSetting);
            }
            else
            {
                shippingRequestBid.price = shippingRequestBid.BasePrice;
            }
        }

        public void AssignShippingRequestActualCommissionAndGoToPostPrice(ShippingRequest shippingRequestItem,TachyonPriceOffer offer)
        {

            offer.OfferStatus = OfferStatus.Accepted;
            shippingRequestItem.Price = offer.TotalAmount;
            shippingRequestItem.Status = ShippingRequestStatus.PostPrice;
           // shippingRequestItem.IsPriceAccepted = true;
            shippingRequestItem.ActualCommissionValue = offer.ActualCommissionValue.Value;
            shippingRequestItem.ActualPercentCommission = offer.ActualPercentCommission.Value;
            shippingRequestItem.ActualMinCommissionValue = offer.ActualMinCommissionValue.Value;
            shippingRequestItem.TotalCommission = offer.TotalCommission.Value;
           /* shippingRequestItem.TachyonDealerProfit = offer.OfferedPrice -
                                                      (shippingRequestItem.CarrierPrice +
                                                       shippingRequestItem.TotalCommission);*/
        }


        /// <summary>
        /// Calculate amount and commission
        /// </summary>
        /// <param name="isTachyonDeal"></param>
        /// <param name="price"></param>
        /// <param name="shippingRequest"></param>
        /// <returns></returns>
        public  Task<ShippingRequestAmount> CalculateAmount(decimal CarrierPrice, ShippingRequest shippingRequest)
        {

            ShippingRequestAmount shippingRequestAmount = new ShippingRequestAmount
            {
                CarrierPrice = CarrierPrice, VatSetting = _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat)
            };
            if (shippingRequest.IsTachyonDeal)
                CalculateAmountByTachyonDealerRequestSettings(shippingRequestAmount, shippingRequest);
            else
                CalculateAmountByShippingRequestSettings(shippingRequestAmount, shippingRequest);

            shippingRequestAmount.TotalCommission = (shippingRequestAmount.CarrierPrice * shippingRequestAmount.PercentCommissionSetting / 100) + shippingRequestAmount.CommissionValueSetting;
            if (shippingRequestAmount.TotalCommission < shippingRequestAmount.MinCommissionValueSetting)
            {
                shippingRequestAmount.TotalCommission = shippingRequestAmount.MinCommissionValueSetting;
                shippingRequestAmount.ActualPercentCommission = 0;
                shippingRequestAmount.ActualCommissionValue = 0;
                shippingRequestAmount.ActualMinCommissionValue = shippingRequestAmount.MinCommissionValueSetting;
            }
            else
            {
                shippingRequestAmount.ActualPercentCommission = shippingRequestAmount.PercentCommissionSetting;
                shippingRequestAmount.ActualCommissionValue = shippingRequestAmount.CommissionValueSetting;
                shippingRequestAmount.ActualMinCommissionValue = 0;
            }
            shippingRequestAmount.SubTotalAmount = shippingRequestAmount.CarrierPrice + shippingRequestAmount.TotalCommission;
            shippingRequestAmount.VatAmount = CalculateVat(shippingRequestAmount.SubTotalAmount, shippingRequestAmount.VatSetting);
            shippingRequestAmount.TotalAmount = shippingRequestAmount.SubTotalAmount + shippingRequestAmount.VatAmount;
            return Task.FromResult(shippingRequestAmount);

        }
        /// <summary>
        /// Calculate total amount based in default settings
        /// </summary>
        /// <param name="CarrierPrice"></param>
        /// <param name="shippingRequest"></param>
        /// <returns></returns>
        public Task<ShippingRequestAmount> CalculateAmountByDefault(decimal CarrierPrice, ShippingRequest shippingRequest)
        {
            ShippingRequestAmount shippingRequestAmount = new ShippingRequestAmount
            {
                CarrierPrice = CarrierPrice,
                VatSetting = _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat)
            };
            if (shippingRequest.IsTachyonDeal)
                CalculateAmountByTachyonDealerRequestSettings(shippingRequestAmount, shippingRequest);
            else
                CalculateAmountByShippingRequestSettings(shippingRequestAmount, shippingRequest);

            shippingRequestAmount.TotalCommission = (shippingRequestAmount.CarrierPrice * shippingRequestAmount.PercentCommissionSetting / 100) + shippingRequestAmount.CommissionValueSetting;
            shippingRequestAmount.ActualPercentCommission = shippingRequestAmount.PercentCommissionSetting;
            shippingRequestAmount.ActualCommissionValue = shippingRequestAmount.CommissionValueSetting;
            //shippingRequestAmount.ActualMinCommissionValue = 0;

            shippingRequestAmount.SubTotalAmount = shippingRequestAmount.CarrierPrice + shippingRequestAmount.TotalCommission;
            shippingRequestAmount.VatAmount = CalculateVat(shippingRequestAmount.SubTotalAmount, shippingRequestAmount.VatSetting);
            shippingRequestAmount.TotalAmount = shippingRequestAmount.SubTotalAmount + shippingRequestAmount.VatAmount;
            return Task.FromResult(shippingRequestAmount);
        }
        /// <summary>
        /// This method calclute commission based on tachyon dealer change the comission settings
        /// </summary>
        /// <param name="tachyonPriceOffer"></param>
        /// <param name="shippingRequest"></param>
        /// <returns></returns>
        public  Task CalculateAmountByOffer(TachyonPriceOffer tachyonPriceOffer, ShippingRequest shippingRequest)
        {
            ShippingRequestAmount shippingRequestAmount = new ShippingRequestAmount();
            CalculateAmountByTachyonDealerRequestSettings(shippingRequestAmount, shippingRequest);
            tachyonPriceOffer.CommissionValueSetting = shippingRequestAmount.CommissionValueSetting;
            tachyonPriceOffer.PercentCommissionSetting = shippingRequestAmount.PercentCommissionSetting;
            tachyonPriceOffer.MinCommissionValueSetting = shippingRequestAmount.MinCommissionValueSetting;
            tachyonPriceOffer.VatSetting = _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);

            //If direct request before get price from carrirer, Apply vat only on the price
            if (!tachyonPriceOffer.CarrirerTenantId.HasValue) {
                tachyonPriceOffer.ActualPercentCommission = default(decimal?);
                tachyonPriceOffer.ActualCommissionValue = default(decimal?);
                tachyonPriceOffer.SubTotalAmount = tachyonPriceOffer.TotalAmount;

                tachyonPriceOffer.VatAmount = CalculateVat(tachyonPriceOffer.SubTotalAmount.Value, tachyonPriceOffer.VatSetting.Value);
                tachyonPriceOffer.TotalAmount = tachyonPriceOffer.SubTotalAmount.Value + tachyonPriceOffer.VatAmount.Value;
                return Task.FromResult(0);
            }


            tachyonPriceOffer.TotalCommission = (tachyonPriceOffer.CarrierPrice.Value * tachyonPriceOffer.ActualPercentCommission.Value / 100) + tachyonPriceOffer.ActualCommissionValue.Value;
            tachyonPriceOffer.SubTotalAmount = tachyonPriceOffer.CarrierPrice + tachyonPriceOffer.TotalCommission;
            tachyonPriceOffer.VatAmount = CalculateVat(tachyonPriceOffer.SubTotalAmount.Value, tachyonPriceOffer.VatSetting.Value);
            tachyonPriceOffer.TotalAmount = tachyonPriceOffer.SubTotalAmount.Value + tachyonPriceOffer.VatAmount.Value;
            return Task.FromResult(0);
        }

        public void CalculateAmountByShippingRequestSettings(ShippingRequestAmount shippingRequestAmount, ShippingRequest request)
        {
            shippingRequestAmount.CommissionValueSetting = Convert.ToDecimal(_featureChecker.GetValue(request.TenantId,AppFeatures.BiddingCommissionValue));
            shippingRequestAmount.PercentCommissionSetting = Convert.ToDecimal(_featureChecker.GetValue(request.TenantId, AppFeatures.BiddingCommissionPercentage));
            shippingRequestAmount.MinCommissionValueSetting = Convert.ToDecimal(_featureChecker.GetValue(request.TenantId, AppFeatures.BiddingMinValueCommission));
            //shippingRequestAmount.CommissionValueSetting = request.CommissionValueSetting;
            //shippingRequestAmount.PercentCommissionSetting = request.PercentCommissionSetting;
            //shippingRequestAmount.MinCommissionValueSetting = request.MinValueCommissionSetting;

        }

        public void CalculateAmountByTachyonDealerRequestSettings(ShippingRequestAmount shippingRequestAmount, ShippingRequest request)
        {
            shippingRequestAmount.CommissionValueSetting = Convert.ToDecimal(_featureChecker.GetValue(request.TenantId, AppFeatures.TachyonDealerCommissionValue));
            shippingRequestAmount.PercentCommissionSetting = Convert.ToDecimal(_featureChecker.GetValue(request.TenantId, AppFeatures.TachyonDealerCommissionPercentage));
            shippingRequestAmount.MinCommissionValueSetting = Convert.ToDecimal(_featureChecker.GetValue(request.TenantId, AppFeatures.TachyonDealerMinValueCommission));

        }
        private async Task<ShippingRequest> GetShippingRequest(long id)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                return await _shippingRequestRepository.FirstOrDefaultAsync(id);

            }
        }

        private decimal CalculateVat(decimal amount, decimal vat)
        {
            return (amount / 100) * vat;
        }

        private decimal CalculatePriceIncludeVat(decimal amount, decimal vat)
        {
            return amount+ CalculateVat(amount, vat);
        }
    }
}
