using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Configuration;
using TACHYON.Features;
using TACHYON.Invoices.Balances;
using TACHYON.Notifications;
using TACHYON.PriceOffers.Dto;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PriceOffers
{
    public class PriceOfferManager : TACHYONDomainServiceBase
    {
        private IRepository<PriceOffer, long> _priceOfferRepository;
        private IRepository<ShippingRequest, long> _shippingRequestsRepository;
        private readonly IRepository<ShippingRequestDirectRequest, long> _shippingRequestDirectRequestRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly ISettingManager _settingManager;
        private readonly IFeatureChecker _featureChecker;
        private readonly IAbpSession _abpSession;
        private readonly BalanceManager _balanceManager;
        private readonly ShippingRequestManager _shippingRequestManager;
        private ShippingRequestDirectRequest _directRequest;

        public PriceOfferManager(IAppNotifier appNotifier, ISettingManager settingManager, IFeatureChecker featureChecker, IRepository<ShippingRequest, long> shippingRequestsRepository, IAbpSession abpSession, BalanceManager balanceManager, ShippingRequestManager shippingRequestManager, IRepository<ShippingRequestDirectRequest, long> shippingRequestDirectRequestRepository, IRepository<PriceOffer, long> priceOfferRepository)
        {
            _appNotifier = appNotifier;
            _settingManager = settingManager;
            _featureChecker = featureChecker;
            _shippingRequestsRepository = shippingRequestsRepository;
            _abpSession = abpSession;
            _balanceManager = balanceManager;
            _shippingRequestManager = shippingRequestManager;
            _shippingRequestDirectRequestRepository = shippingRequestDirectRequestRepository;
            _priceOfferRepository = priceOfferRepository;
        }

        public async Task CreateOrEdit(CreateOrEditPriceOfferInput Input)
        {
            DisableTenancyFilters();

            var shippingRequest = await _shippingRequestsRepository
                                        .GetAll()
                                        .Include(x => x.ShippingRequestVases)
                                        .FirstOrDefaultAsync(r => r.Id == Input.ShippingRequestId && (r.Status == ShippingRequestStatus.NeedsAction || r.Status == ShippingRequestStatus.PrePrice));
            if (shippingRequest == null) throw new UserFriendlyException(L("TheShippingRequestNotFound"));

            if (Input.Channel == PriceOfferChannel.MarketPlace)
            {
                MarketPlaceCanAccess(Input, shippingRequest);
            }
            else
            {
                await DirectRequestCanAccess(Input, shippingRequest);
            }
            var offer = GetCarrierPricingOrNull(Input.ShippingRequestId);
            if (offer == null)
            {
                await Create(Input, shippingRequest);
            }
            else
            {
                await Update(Input, shippingRequest, offer);
            }
        }

        public async Task Delete(EntityDto Input)
        {
            DisableTenancyFilters();
            var pricing = await _priceOfferRepository
                .GetAll()
                .Include(x => x.ShippingRequestFK)
                .FirstOrDefaultAsync(x => x.Id == Input.Id && x.TenantId == _abpSession.TenantId.Value && x.ShippingRequestFK.Status == ShippingRequestStatus.NeedsAction);
            if (pricing == null) throw new UserFriendlyException(L("TheRecordNotFound"));
            var request = pricing.ShippingRequestFK;
            if (pricing.Channel == PriceOfferChannel.MarketPlace)
            {
                if (pricing.ShippingRequestFK.BidStatus != ShippingRequestBidStatus.OnGoing) throw new UserFriendlyException(L("TheRecordNotFound"));
                request.TotalBids -= 1;
            }
            else if (pricing.Channel == PriceOfferChannel.DirectRequest)
            {
                var directRequest = await _shippingRequestDirectRequestRepository.FirstOrDefaultAsync(x => x.Id == pricing.SourceId);
                if (directRequest != null) directRequest.Status = ShippingRequestDirectRequestStatus.New;
            }

            if (!await _priceOfferRepository.GetAll().AnyAsync(x => x.ShippingRequestId == request.Id && x.Id != Input.Id))
            {
                request.Status = ShippingRequestStatus.PrePrice;
            }
            await _priceOfferRepository.DeleteAsync(pricing);

        }


        //Shipper Accept Offer
        public async Task AcceptOffer(long Id)
        {
            DisableTenancyFilters();
            var offer = await _priceOfferRepository
                .GetAll()
                .Include(x => x.ShippingRequestFK)
                .Include(t => t.Tenant)
                  .ThenInclude(e => e.Edition)
                .FirstOrDefaultAsync(x => x.Id == Id &&
                x.ShippingRequestFK.TenantId == _abpSession.TenantId.Value &&
                x.Status == PriceOfferStatus.New &&
                x.ShippingRequestFK.Status == ShippingRequestStatus.NeedsAction &&
                (!x.ShippingRequestFK.IsTachyonDeal || (x.ShippingRequestFK.IsTachyonDeal && x.ParentId.HasValue)));
            if (offer == null) throw new UserFriendlyException(L("TheOfferIsNotFound"));


            var request = offer.ShippingRequestFK;
            var edtion = offer.Tenant.Edition;
            PriceOffer parentOffer = default;
            //Check if shipper have enough balance to pay 
            await _balanceManager.ShipperCanAcceptOffer(offer);

            List<UserIdentifier> Users = new List<UserIdentifier>();
            /// Check if the offer send by TAD on market place
            if (edtion.Name.ToLower() == AppConsts.TachyonEditionName && offer.Channel == PriceOfferChannel.MarketPlace)
            {
                offer.Status = PriceOfferStatus.AcceptedAndWaitingForCarrier;
                request.Status = ShippingRequestStatus.AcceptedAndWaitingCarrier;
                request.IsBid = false;
                request.BidStatus = ShippingRequestBidStatus.StandBy;
                request.BidStartDate = default;
                request.BidEndDate = default;
            }
            /// Check if offer has carrier from parent offer comming
            else if (offer.ParentId.HasValue || !offer.ShippingRequestFK.IsTachyonDeal)
            {
                offer.Status = PriceOfferStatus.Accepted;
                request.Status = ShippingRequestStatus.Completed;
                if (offer.ParentId.HasValue)
                {
                    parentOffer = await GetOfferById(offer.ParentId.Value);
                }
                request.CarrierTenantId = parentOffer != null ? parentOffer.TenantId : offer.TenantId;
                if (request.IsBid) request.BidStatus = ShippingRequestBidStatus.Closed;
            }
            else //TAD still need to find carrier ro assign to shipping request
            {
                offer.Status = PriceOfferStatus.AcceptedAndWaitingForCarrier;
                request.Status = ShippingRequestStatus.AcceptedAndWaitingCarrier;

            }

            await SetShippingRequestPricing(offer);
            await _appNotifier.ShipperAcceptedOffers(offer, parentOffer);
        }
        /// <summary>
        /// Get parent offer when the current offer is tachyon deal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public async Task<PriceOffer> GetOfferById(long id)
        {
            return (await _priceOfferRepository.FirstOrDefaultAsync(x => x.Id == id));
        }

        public async Task<PriceOffer> GetOfferBySource(long sourceId, PriceOfferChannel channel)
        {
            return (await _priceOfferRepository.FirstOrDefaultAsync(x => x.SourceId == sourceId && x.Channel == channel));
        }
        #region Helper



        /// <summary>
        /// Find carrier tenant id to assing to shipping request if the offer direct from carrier or by TAD
        /// </summary>
        /// <param name="offer"></param>
        /// <returns></returns>
        private async Task<int> GetCarrierTenantId(ShippingRequestPricing offer)
        {
            if (!offer.ShippingRequestFK.IsTachyonDeal) return offer.TenantId;
            return (await _priceOfferRepository.FirstOrDefaultAsync(x => x.Id == offer.ParentId)).TenantId;
        }
        /// <summary>
        /// Set the shipping reqquest prices
        /// </summary>
        /// <param name="offer"></param>
        /// <returns></returns>
        private Task SetShippingRequestPricing(PriceOffer offer)
        {
            var request = offer.ShippingRequestFK;
            request.Price = offer.TotalAmountWithCommission;
            request.SubTotalAmount = offer.SubTotalAmountWithCommission;
            request.VatAmount = offer.VatAmountWithCommission;
            request.VatSetting = offer.TaxVat;
            request.TotalCommission = offer.TotalAmountWithCommission;

            return Task.CompletedTask;
        }
        /// <summary>
        /// If the current user login have feature carrier get the offer 
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public PriceOffer GetCarrierPricingOrNull(long requestId)
        {
            return _priceOfferRepository.GetAll().Include(x => x.PriceOfferDetails).FirstOrDefault(x => x.ShippingRequestId == requestId && x.TenantId == _abpSession.TenantId.Value);
        }
        public bool CheckCarrierIsPricing(long requestId)
        {
            return _priceOfferRepository.GetAll().Any(x => x.ShippingRequestId == requestId && x.TenantId == _abpSession.TenantId.Value);
        }
        /// <summary>
        /// Check If the shipping request in market place to access to add offer
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="shippingRequest"></param>
        private void MarketPlaceCanAccess(CreateOrEditPriceOfferInput Input, ShippingRequest shippingRequest)
        {
            if (shippingRequest.BidStatus != ShippingRequestBidStatus.OnGoing) throw new UserFriendlyException(L("The Bid must be Ongoing"));
            if (_featureChecker.IsEnabled(AppFeatures.TachyonDealer) && shippingRequest.IsTachyonDeal) throw new UserFriendlyException(L("YouCanNotMakeBidFromMarketPlaceWhenTheShippingManageByTAD"));

        }

        /// <summary>
        /// Check the carrier of have direct request to access to add offer
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="shippingRequest"></param>
        /// <returns></returns>
        private async Task DirectRequestCanAccess(CreateOrEditPriceOfferInput Input, ShippingRequest shippingRequest)
        {
            _directRequest = await _shippingRequestDirectRequestRepository.FirstOrDefaultAsync(x => x.CarrierTenantId == _abpSession.TenantId.Value && x.ShippingRequestId == Input.ShippingRequestId);
            if (_directRequest == null) throw new UserFriendlyException(L("YouDoNotHaveDirectRequest"));
            if (_directRequest.Status == ShippingRequestDirectRequestStatus.Accepted) throw new UserFriendlyException(L("YouCanNoCreateOrEditPriceWhenThePriceAccepted"));
            Input.SourceId = _directRequest.Id;
        }
        private async Task Create(CreateOrEditPriceOfferInput Input, ShippingRequest shippingRequest)
        {

            var Pricing = ObjectMapper.Map<PriceOffer>(Input);
            Pricing.PriceOfferDetails = await GetListOfVases(Input, shippingRequest);
            Pricing.ShippingRequestFK = shippingRequest;
            Calculate(Pricing, shippingRequest);
            Pricing.Id = await _priceOfferRepository.InsertAndGetIdAsync(Pricing);
            if (shippingRequest.Status != ShippingRequestStatus.NeedsAction) shippingRequest.Status = ShippingRequestStatus.NeedsAction;
            if (Pricing.Channel == PriceOfferChannel.MarketPlace) shippingRequest.TotalBids += 1;
            if (Pricing.Channel == PriceOfferChannel.DirectRequest) _directRequest.Status = ShippingRequestDirectRequestStatus.Response;
            await _appNotifier.ShippingRequestSendOfferWhenAddPrice(Pricing, GetCurrentTenant(_abpSession).Name);

        }
        private async Task Update(CreateOrEditPriceOfferInput Input, ShippingRequest shippingRequest, PriceOffer Pricing)
        {
            if (Pricing.Status == PriceOfferStatus.Accepted) throw new UserFriendlyException(L("YourPriceAlreadyAcceptedYouCanEdit"));
            ObjectMapper.Map(Input, Pricing);
            Pricing.PriceOfferDetails.Clear();
            Pricing.PriceOfferDetails = await GetListOfVases(Input, shippingRequest);
            Calculate(Pricing, shippingRequest);
            if (Pricing.IsView)
            {
                Pricing.IsView = false;
                Pricing.ShippingRequestFK = shippingRequest;
                await  _appNotifier.ShippingRequestSendOfferWhenUpdatePrice(Pricing, GetCurrentTenant(_abpSession).Name);

            }
            await _priceOfferRepository.UpdateAsync(Pricing);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="shippingRequest"></param>
        /// <returns></returns>
        private Task<List<PriceOfferDetail>> GetListOfVases(CreateOrEditPriceOfferInput Input, ShippingRequest shippingRequest)
        {
            List<PriceOfferDetail> ShippingRequestVasesPricing = new List<PriceOfferDetail>();
            if (Input.ItemDetails.Count != shippingRequest.ShippingRequestVases.Count)
            {
                throw new UserFriendlyException(L("YouSholudAddPricesForAllVases"));
            }
            if (Input.ItemDetails.Any(x => x.Price < 0)) throw new UserFriendlyException(L("ThePriceMustBeGreaterThanZero"));
            foreach (var vas in shippingRequest.ShippingRequestVases)
            {
                var vasdto = Input.ItemDetails.FirstOrDefault(x => x.ItemId == vas.Id);
                if (vasdto == null) throw new UserFriendlyException(L("YouSholudAddVasRelatedWithShippingRequest"));

                ShippingRequestVasesPricing.Add(new PriceOfferDetail()
                {
                    ItemPrice = vasdto.Price,
                    PriceType= PriceOfferType.Vas,
                    Quantity= vas.RequestMaxCount+ vas.RequestMaxAmount
                }); ;
            }
            return Task.FromResult(ShippingRequestVasesPricing);
        }



        #region Calculate
        private decimal _minValueCommission;

        private decimal _detailMinValueCommission;
        private void Calculate(PriceOffer offer, ShippingRequest shippingRequest)
        {
            offer.TaxVat = _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);
            SetCalculateSettings(offer, shippingRequest);
            CalculatePriceOfferDetails(offer);
            CalculateSingle(offer);
            CalculateFinalPrices(offer);
            CalculateSingleWithCommission(offer);
            CalculateMultipleFinalPricesWithCommission(offer);
        }

        private void SetCalculateSettings(PriceOffer offer, ShippingRequest shippingRequest)
        {
            switch (offer.PriceType)
            {
                case PriceOfferType.Trip:
                    SetCalculateTripSettings(offer, shippingRequest);
                    break;
            }

        }

        private void SetCalculateTripSettings(PriceOffer offer, ShippingRequest shippingRequest)
        {
            PriceOfferCommissionType vasCommissionType;
            decimal vasCommissionPercentageOrAddValue=0;
            offer.Quantity = shippingRequest.NumberOfTrips;
            if (shippingRequest.IsTachyonDeal)
            {
                offer.CommissionType = (PriceOfferCommissionType)Convert.ToByte(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerTripCommissionType));
                _minValueCommission = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerTripMinValueCommission));
                vasCommissionType = (PriceOfferCommissionType)Convert.ToByte(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerVasCommissionType));
                _detailMinValueCommission = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerVasMinValueCommission));

                switch (offer.CommissionType)
                {
                    case PriceOfferCommissionType.Percentage:
                        offer.CommissionPercentageOrAddValue = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerTripCommissionPercentage));
                        vasCommissionPercentageOrAddValue = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerVasCommissionPercentage));
                        break;
                    case PriceOfferCommissionType.Value:
                        offer.CommissionPercentageOrAddValue = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerTripCommissionValue));
                        vasCommissionPercentageOrAddValue = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerVasCommissionValue));

                        break;
                }
            }
            else
            {
                offer.CommissionType = (PriceOfferCommissionType)Convert.ToByte(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TripCommissionType));
                _minValueCommission = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TripMinValueCommission));
                vasCommissionType = (PriceOfferCommissionType)Convert.ToByte(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.VasCommissionType));
                _detailMinValueCommission = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.VasMinValueCommission));
                switch (offer.CommissionType)
                {
                    case PriceOfferCommissionType.Percentage:
                        offer.CommissionPercentageOrAddValue = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TripCommissionPercentage));
                        vasCommissionPercentageOrAddValue = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.VasCommissionPercentage));

                        break;
                    case PriceOfferCommissionType.Value:
                        offer.CommissionPercentageOrAddValue = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TripCommissionValue));
                        vasCommissionPercentageOrAddValue = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.VasCommissionValue));

                        break;
                }
            }
            SetPriceOfferDetailsCommissionSettings(offer, vasCommissionType, vasCommissionPercentageOrAddValue);
            CalculateCommission(offer);
        }



        private void CalculateCommission(PriceOffer offer)
        {
            switch (offer.CommissionType)
            {
                case PriceOfferCommissionType.Percentage:
                    offer.ItemCommissionAmount = (offer.ItemPrice * offer.CommissionPercentageOrAddValue / 100);
                    if (offer.ItemCommissionAmount < _minValueCommission)
                    {
                        offer.ItemCommissionAmount = _minValueCommission;
                        offer.CommissionType = PriceOfferCommissionType.MinValue ;
                    }
                    break;
                case PriceOfferCommissionType.Value:
                    offer.ItemCommissionAmount = offer.ItemPrice + offer.CommissionPercentageOrAddValue;
                    break;
            }
        }


        private void CalculateSingle(PriceOffer offer)
        {
            offer.ItemVatAmount = TACHYON.Common.Calculate.CalculateVat(offer.ItemPrice, offer.TaxVat);
            offer.ItemTotalAmount = offer.ItemPrice + offer.ItemVatAmount;

        }

        private void CalculateFinalPrices(PriceOffer offer)
        {
            offer.SubTotalAmount = (offer.ItemPrice * offer.Quantity) + offer.PriceOfferDetails.Sum(x => x.SubTotalAmount);
            offer.VatAmount = (offer.ItemVatAmount * offer.Quantity) + offer.PriceOfferDetails.Sum(x => x.VatAmount);
            offer.TotalAmount = (offer.ItemTotalAmount * offer.Quantity) + offer.PriceOfferDetails.Sum(x => x.TotalAmount);
        }
        private void CalculateSingleWithCommission(PriceOffer offer)
        {
            offer.ItemSubTotalAmountWithCommission = offer.ItemPrice + offer.ItemCommissionAmount;
            offer.ItemVatAmountWithCommission = TACHYON.Common.Calculate.CalculateVat(offer.ItemSubTotalAmountWithCommission, offer.TaxVat);
            offer.ItemTotalAmountWithCommission = offer.ItemSubTotalAmountWithCommission + offer.ItemVatAmountWithCommission;

        }
        private void CalculateMultipleFinalPricesWithCommission(PriceOffer offer)
        {
            offer.SubTotalAmountWithCommission = (offer.ItemSubTotalAmountWithCommission * offer.Quantity) + offer.PriceOfferDetails.Sum(x => x.SubTotalAmountWithCommission);
            offer.VatAmountWithCommission = (offer.ItemVatAmountWithCommission * offer.Quantity) + offer.PriceOfferDetails.Sum(x => x.VatAmountWithCommission);
            offer.TotalAmountWithCommission = (offer.ItemTotalAmountWithCommission * offer.Quantity) + offer.PriceOfferDetails.Sum(x => x.TotalAmountWithCommission);
            offer.CommissionAmount = (offer.ItemCommissionAmount * offer.Quantity) + offer.PriceOfferDetails.Sum(x => x.CommissionAmount);
        }

        #region Details
        private void SetPriceOfferDetailsCommissionSettings(PriceOffer offer, PriceOfferCommissionType commissionType, decimal commissionPercentageOrAddValue)
        {
            if (offer.PriceOfferDetails == null)
            {
                offer.PriceOfferDetails = new List<PriceOfferDetail>();
                return;
            }
            foreach (var item in offer.PriceOfferDetails)
            {
                item.CommissionType = commissionType;
                item.CommissionPercentageOrAddValue = commissionPercentageOrAddValue;
            }
        }
        private void CalculatePriceOfferDetails(PriceOffer offer)
        {

            foreach (var item in offer.PriceOfferDetails)
            {
                CalculateDetailCommission(offer, item);
                CalculateSingleDetail(offer, item);
                CalculateMultipleDetails(item);
                CalculateSingleDetailWithCommission(offer, item);
                CalculateMultipleDetailsWithCommission(item);
            }
        }

        private void CalculateDetailCommission(PriceOffer offer,PriceOfferDetail item)
        {
            switch (item.CommissionType)
            {
                case PriceOfferCommissionType.Percentage:
                    item.ItemCommissionAmount = (item.ItemPrice * item.CommissionPercentageOrAddValue / 100);

                    if (item.ItemCommissionAmount < _detailMinValueCommission)
                    {
                        item.ItemCommissionAmount = _detailMinValueCommission;
                        item.CommissionType = PriceOfferCommissionType.MinValue;
                    }
                    break;
                case PriceOfferCommissionType.Value:
                    item.ItemCommissionAmount = item.ItemPrice + item.CommissionPercentageOrAddValue;
                    break;
            }
        }

        private void CalculateSingleDetail(PriceOffer offer, PriceOfferDetail item)
        {
            item.ItemVatAmount = TACHYON.Common.Calculate.CalculateVat(item.ItemPrice, offer.TaxVat);
            item.ItemTotalAmount = item.ItemPrice + item.ItemVatAmount;

        }
        private void CalculateMultipleDetails(PriceOfferDetail item)
        {
            item.SubTotalAmount = item.ItemPrice * item.Quantity;
            item.TotalAmount = item.ItemTotalAmount * item.Quantity;
            item.VatAmount = item.ItemVatAmount * item.Quantity;
        }

        private void CalculateSingleDetailWithCommission(PriceOffer offer, PriceOfferDetail item)
        {
            item.ItemSubTotalAmountWithCommission = item.ItemPrice + item.ItemCommissionAmount;
            item.ItemVatAmountWithCommission = TACHYON.Common.Calculate.CalculateVat(item.ItemSubTotalAmountWithCommission, offer.TaxVat);
            item.ItemTotalAmountWithCommission = item.ItemSubTotalAmountWithCommission + item.ItemVatAmountWithCommission;

        }
        private void CalculateMultipleDetailsWithCommission(PriceOfferDetail item)
        {
            item.SubTotalAmountWithCommission = item.ItemSubTotalAmountWithCommission * item.Quantity;
            item.VatAmountWithCommission = item.ItemVatAmountWithCommission * item.Quantity;
            item.TotalAmountWithCommission = item.ItemTotalAmountWithCommission * item.Quantity;
            item.CommissionAmount = item.ItemCommissionAmount * item.Quantity;
        }
        #endregion
        #endregion

        #endregion
    }

}
