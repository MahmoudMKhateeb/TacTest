using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
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
        private ShippingRequestDirectRequest _directRequest;

        public PriceOfferManager(IAppNotifier appNotifier, ISettingManager settingManager, IFeatureChecker featureChecker, IRepository<ShippingRequest, long> shippingRequestsRepository, IAbpSession abpSession, BalanceManager balanceManager, IRepository<ShippingRequestDirectRequest, long> shippingRequestDirectRequestRepository, IRepository<PriceOffer, long> priceOfferRepository)
        {
            _appNotifier = appNotifier;
            _settingManager = settingManager;
            _featureChecker = featureChecker;
            _shippingRequestsRepository = shippingRequestsRepository;
            _abpSession = abpSession;
            _balanceManager = balanceManager;
            _shippingRequestDirectRequestRepository = shippingRequestDirectRequestRepository;
            _priceOfferRepository = priceOfferRepository;
        }
        /// <summary>
        /// Create Or Edit Offer
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public async Task<long> CreateOrEdit(CreateOrEditPriceOfferInput Input)
        {
            DisableTenancyFilters();

            var shippingRequest = await _shippingRequestsRepository
                                        .GetAll()
                                        .Include(x => x.ShippingRequestVases)
                                        .FirstOrDefaultAsync(r => r.Id == Input.ShippingRequestId && (r.Status == ShippingRequestStatus.NeedsAction || r.Status == ShippingRequestStatus.PrePrice || r.Status == ShippingRequestStatus.AcceptedAndWaitingCarrier));
            if (shippingRequest == null) throw new UserFriendlyException(L("TheShippingRequestNotFound"));


            if (await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer) && shippingRequest.IsTachyonDeal)
            {
                Input.Channel = PriceOfferChannel.TachyonManageService;
            }

            if (Input.Channel == PriceOfferChannel.MarketPlace)
            {
                MarketPlaceCanAccess(Input, shippingRequest);
            }
            if (_featureChecker.IsEnabled(AppFeatures.TachyonDealer)) Input.Channel = PriceOfferChannel.TachyonManageService;

            if (Input.Channel == PriceOfferChannel.DirectRequest)
            {
                await DirectRequestCanAccess(Input, shippingRequest);
            }
            var offer = GetCarrierPricingOrNull(Input.ShippingRequestId);
            
            if (offer == null || offer.Status == PriceOfferStatus.Rejected)
            {
              
                return await Create(Input, shippingRequest, offer);
            }
            else
            {
                return await Update(Input, shippingRequest, offer);
            }
        }

        /// <summary>
        /// Delete the offer when status the shipping request is needs action
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public async Task Delete(EntityDto Input)
        {
            DisableTenancyFilters();
            var pricing = await _priceOfferRepository
                .GetAll()
                .Include(x => x.ShippingRequestFK)
                .FirstOrDefaultAsync(x => x.Id == Input.Id && (x.TenantId == _abpSession.TenantId.Value || !_abpSession.TenantId.HasValue) && (x.ShippingRequestFK.Status == ShippingRequestStatus.NeedsAction || x.ShippingRequestFK.Status == ShippingRequestStatus.AcceptedAndWaitingCarrier));
            if (pricing == null) throw new UserFriendlyException(L("TheRecordNotFound"));
            var request = pricing.ShippingRequestFK;
            if (pricing.Channel == PriceOfferChannel.MarketPlace)
            {
                if (pricing.ShippingRequestFK.BidStatus != ShippingRequestBidStatus.OnGoing) throw new UserFriendlyException(L("TheRecordNotFound"));
                request.TotalOffers -= 1;
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


        //Shipper Or TAD accept carrirer offer Accept Offer
        public async Task<PriceOfferStatus> AcceptOffer(long id)
        {
            DisableTenancyFilters();

            var offer = await CanAcceptOrRejectOffer(id);
            if (offer == null) throw new UserFriendlyException(L("YouCanNotAcceptTheOffer"));
            await CheckIfThereOfferAcceptedBefore(offer.ShippingRequestId);
            if (!_abpSession.TenantId.HasValue || _featureChecker.IsEnabled(AppFeatures.TachyonDealer))
            {
                await TachyonAcceptOffer(offer);
                return offer.Status ;
            }

            var request = offer.ShippingRequestFK;

            PriceOffer parentOffer = default;
            //Check if shipper have enough balance to pay 
            await _balanceManager.ShipperCanAcceptOffer(offer);

            List<UserIdentifier> Users = new List<UserIdentifier>();
            /// Check if the offer send by TAD on market place
            if (offer.Tenant.EditionId == AppConsts.TachyonEditionId && !offer.ShippingRequestFK.IsTachyonDeal)
            {
                offer.Status = PriceOfferStatus.AcceptedAndWaitingForCarrier;
                request.IsBid = false;
                request.BidStatus = ShippingRequestBidStatus.StandBy;
                request.BidStartDate = default;
                request.BidEndDate = default;
                request.IsTachyonDeal = true;

            }
            /// Check if offer has carrier from parent offer coming
            else if (offer.ParentId.HasValue || !offer.ShippingRequestFK.IsTachyonDeal)
            {
                offer.Status = PriceOfferStatus.Accepted;
                request.Status = ShippingRequestStatus.PostPrice;
                if (offer.ParentId.HasValue)
                {
                    parentOffer = await GetOfferById(offer.ParentId.Value);
                }
                request.CarrierTenantId = parentOffer?.TenantId ?? offer.TenantId;
                if (request.IsBid) request.BidStatus = ShippingRequestBidStatus.Closed;

                if (parentOffer !=null && parentOffer.Channel== PriceOfferChannel.DirectRequest)
                    await ChangeDirectRequestStatus(parentOffer.SourceId.Value, ShippingRequestDirectRequestStatus.Accepted);
                if (offer.Channel == PriceOfferChannel.DirectRequest)
                    await ChangeDirectRequestStatus(offer.SourceId.Value, ShippingRequestDirectRequestStatus.Accepted);

              if (parentOffer !=null)  await _appNotifier.TMSAcceptedOffer(parentOffer);


            }
            else //TAD still need to find carrier to assign to shipping request
            {
                offer.Status = PriceOfferStatus.AcceptedAndWaitingForCarrier;
                //request.Status = ShippingRequestStatus.AcceptedAndWaitingCarrier;

            }

            await SetShippingRequestPricing(offer);
            await _appNotifier.ShipperAcceptedOffer(offer);
            return offer.Status;
        }
        /// <summary>
        /// If the offer managed by TMS
        /// </summary>
        /// <param name="offer"></param>
        /// <returns></returns>
        private async Task TachyonAcceptOffer(PriceOffer offer)
        {
            if (!_abpSession.TenantId.HasValue || _featureChecker.IsEnabled(AppFeatures.TachyonDealer))
            {
               var exisistOffer=  await _priceOfferRepository
                      .GetAll()
                      .Where(x => x.ShippingRequestId == offer.ShippingRequestId &&  x.Status == PriceOfferStatus.AcceptedAndWaitingForCarrier)
                      .FirstOrDefaultAsync();
                if (exisistOffer != null) /// If true then have exists offer approve by shipper
                {
                   CheckIfPriceLowerThanSendByTMS(exisistOffer.TotalAmount, offer.TotalAmount);

                    exisistOffer.ParentId = offer.Id;
                    exisistOffer.Status = PriceOfferStatus.Accepted;
                    offer.Status = PriceOfferStatus.Accepted;
                    offer.ShippingRequestFK.Status = ShippingRequestStatus.PostPrice;
                    if (offer.Channel == PriceOfferChannel.DirectRequest)
                    {
                        await ChangeDirectRequestStatus(offer.SourceId.Value, ShippingRequestDirectRequestStatus.Accepted);
                    }
                    offer.ShippingRequestFK.CarrierTenantId =  offer.TenantId;
                    if (offer.ShippingRequestFK.IsBid) offer.ShippingRequestFK.BidStatus = ShippingRequestBidStatus.Closed;

                    await _appNotifier.TMSAcceptedOffer(offer);
                }
                else 
                {
                    var TADOffer = await _priceOfferRepository
                      .GetAll()
                      .Where(x => x.ShippingRequestId == offer.ShippingRequestId &&
                      x.Tenant.EditionId == AppConsts.TachyonEditionId  &&
                      (x.Status == PriceOfferStatus.New || x.Status == PriceOfferStatus.Rejected))
                     .FirstOrDefaultAsync();


                    if (TADOffer != null) /// If TMS send offer before accept carrier price then related offers togother
                    {
                        CheckIfPriceLowerThanSendByTMS(TADOffer.TotalAmount , offer.TotalAmount);
                        TADOffer.ParentId = offer.Id;
                        TADOffer.Status = PriceOfferStatus.AcceptedAndWaitingForShipper;

                    }

                    offer.Status = PriceOfferStatus.Pending;
                    if (offer.Channel == PriceOfferChannel.DirectRequest)
                    {
                        await ChangeDirectRequestStatus(offer.SourceId.Value, ShippingRequestDirectRequestStatus.Pending);
                    }
                   await _appNotifier.PendingOffer(offer);
                }
            }
        }
        /// <summary>
        /// If the TMS send price before to shipper then check if the price lower than carrier accepted 
        /// </summary>
        /// <param name="TMSTotalAmount"></param>
        /// <param name="CarrierTotalAmount"></param>
        private void CheckIfPriceLowerThanSendByTMS(decimal TMSTotalAmount,decimal CarrierTotalAmount)
        {
            if (TMSTotalAmount < CarrierTotalAmount)
                throw new UserFriendlyException(L("YouCannotAcceptLowerThanPriceSentToShipper"));
        }
        /// <summary>
        /// Reject offer
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task RejectOffer(RejectPriceOfferInput input)
        {
            DisableTenancyFilters();

            var offer = await CanAcceptOrRejectOffer(input.Id);
            if (offer == null) throw new UserFriendlyException(L("YouCanNotRejectTheOffer"));
            await CheckIfThereOfferAcceptedBefore(offer.ShippingRequestId);
            offer.Status = PriceOfferStatus.Rejected;
            offer.RejectedReason = input.Reason;
            if (offer.Channel == PriceOfferChannel.DirectRequest)
            {
                var directRequest = await _shippingRequestDirectRequestRepository.FirstOrDefaultAsync(x => x.Id == offer.SourceId);
                if (directRequest != null) directRequest.Status = ShippingRequestDirectRequestStatus.Rejected;
            }
           await _appNotifier.RejectedOffer(offer, input.RejectBy);
        }
        private async Task<PriceOffer> CanAcceptOrRejectOffer(long id)
        {
            return await _priceOfferRepository
                  .GetAll()
                  .Include(r => r.ShippingRequestFK)
                   .ThenInclude(r => r.Tenant)
                  .Include(t=>t.Tenant)
                  .Where(x => x.Id == id && (x.ShippingRequestFK.Status == ShippingRequestStatus.NeedsAction || x.ShippingRequestFK.Status == ShippingRequestStatus.AcceptedAndWaitingCarrier))
                  .WhereIf(_abpSession.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFK.TenantId == _abpSession.TenantId && 
                  (!x.ShippingRequestFK.IsTachyonDeal || x.Channel== PriceOfferChannel.TachyonManageService ) &&
                  (x.Status == PriceOfferStatus.New || x.Status == PriceOfferStatus.AcceptedAndWaitingForShipper))
                  .WhereIf(!_abpSession.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestFK.IsTachyonDeal && x.Tenant.EditionId != 4 && x.Status == PriceOfferStatus.New)
                  .FirstOrDefaultAsync();
        }
        private async Task CheckIfThereOfferAcceptedBefore(long id)
        {
            var offer=await _priceOfferRepository
                  .GetAll()
                  .Include(r => r.ShippingRequestFK)
                  .Where(x => x.ShippingRequestId == id)
                  .WhereIf(_abpSession.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Shipper),
                  x => x.ShippingRequestFK.TenantId == _abpSession.TenantId  && x.Status== PriceOfferStatus.AcceptedAndWaitingForCarrier)
                  .WhereIf(!_abpSession.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestFK.IsTachyonDeal && (x.Status== PriceOfferStatus.AcceptedAndWaitingForShipper || x.Status== PriceOfferStatus.Pending) )
                  .FirstOrDefaultAsync();
            if (offer != null) throw new UserFriendlyException(L("YouAlreadyAcceptedOfferBefore"));
            
        }
        /// <summary>
        /// Return offer by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public async Task<PriceOffer> GetOfferById(long id)
        {
            return (await _priceOfferRepository.FirstOrDefaultAsync(x => x.Id == id));
        }

        /// <summary>
        /// Return offer by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public async Task<PriceOffer> GetOffercceptedByShippingRequestId(long id)
        {
            return (await _priceOfferRepository
                .GetAllIncluding(x=>x.PriceOfferDetails)
                .FirstOrDefaultAsync(x => x.ShippingRequestId == id && x.Status== PriceOfferStatus.Accepted));
        }

        /// <summary>
        /// Return offer by parent id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public async Task<PriceOffer> GetOfferParentId(long id)
        {
            return (await _priceOfferRepository.FirstOrDefaultAsync(x => x.ParentId == id));
        }
        /// <summary>
        /// Return offer by source id and channel
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public async Task<PriceOffer> GetOfferBySource(long sourceId, PriceOfferChannel channel)
        {
            return (await _priceOfferRepository.GetAll().OrderByDescending(x=>x.Id).FirstOrDefaultAsync(x => x.SourceId == sourceId && x.Channel == channel));
        }
        #region Helper
        /// <summary>
        /// Load offer pricing approve by shipper in shipment
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
            return _priceOfferRepository
                .GetAll()
                .Include(x => x.PriceOfferDetails)
                .OrderByDescending(x=> x.Id)
                .FirstOrDefault(x => x.ShippingRequestId == requestId && x.TenantId == _abpSession.TenantId.Value && x.Status != PriceOfferStatus.ForceRejected);
        }
        /// <summary>
        /// Check if the carrier already make pricing for the shipment or not
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
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
            //if (_featureChecker.IsEnabled(AppFeatures.TachyonDealer) && shippingRequest.IsTachyonDeal) throw new UserFriendlyException(L("YouCanNotMakeBidFromMarketPlaceWhenTheShippingManageByTAD"));

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
        /// <summary>
        /// Change the direct request status
        /// </summary>
        /// <param name="directRequestId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task ChangeDirectRequestStatus(long directRequestId,ShippingRequestDirectRequestStatus status)
        {
            _directRequest = await _shippingRequestDirectRequestRepository.FirstOrDefaultAsync(x => x.Id== directRequestId);
            _directRequest.Status = status;
        }
        /// <summary>
        /// Create offer
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="shippingRequest"></param>
        /// <returns></returns>
        private async Task<long> Create(CreateOrEditPriceOfferInput Input, ShippingRequest shippingRequest,PriceOffer RejectedOffer)
        {

            var offer = ObjectMapper.Map<PriceOffer>(Input);
            offer.PriceOfferDetails = await GetListOfVases(Input, shippingRequest);
            offer.ShippingRequestFK = shippingRequest;
            Calculate(offer, shippingRequest,Input);

            if (RejectedOffer !=null) /// If the user re priced the price from rejected offer then change the status for prevoius rejected to forcerejected
            {
                if (RejectedOffer.ParentId.HasValue) offer.ParentId = RejectedOffer.ParentId;/// if the rejected related with parent offer set the parent offer for new offer created
                    RejectedOffer.Status = PriceOfferStatus.ForceRejected;
            }

            offer.Id = await _priceOfferRepository.InsertAndGetIdAsync(offer);
            if (shippingRequest.Status != ShippingRequestStatus.NeedsAction) shippingRequest.Status = ShippingRequestStatus.NeedsAction;
            shippingRequest.TotalOffers += 1;
            if (offer.Channel == PriceOfferChannel.DirectRequest) _directRequest.Status = ShippingRequestDirectRequestStatus.Response;

            if (offer.ParentId != null) offer.Status = PriceOfferStatus.AcceptedAndWaitingForShipper;
            await _appNotifier.ShippingRequestSendOfferWhenAddPrice(offer, GetCurrentTenant(_abpSession).Name);
            return offer.Id;

        }
        /// <summary>
        /// Update offer
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="shippingRequest"></param>
        /// <param name="Pricing"></param>
        /// <returns></returns>
        private async Task<long> Update(CreateOrEditPriceOfferInput Input, ShippingRequest shippingRequest, PriceOffer offer)
        {
            if (offer.Status == PriceOfferStatus.Accepted) throw new UserFriendlyException(L("YourPriceAlreadyAcceptedYouCanEdit"));
            var Channel = offer.Channel;
            ObjectMapper.Map(Input, offer);
            offer.Channel = Channel;
            offer.PriceOfferDetails.Clear();
            offer.PriceOfferDetails = await GetListOfVases(Input, shippingRequest);
            Calculate(offer, shippingRequest, Input);
            if (offer.IsView)
            {
                offer.IsView = false;
                offer.ShippingRequestFK = shippingRequest;
                await  _appNotifier.ShippingRequestSendOfferWhenUpdatePrice(offer, GetCurrentTenant(_abpSession).Name);

            }
            await _priceOfferRepository.UpdateAsync(offer);
            return offer.Id;
        }
        /// <summary>
        /// Get list of vases after merege data between shipemnt vases and input vases
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
                    SourceId = vas.Id,
                    ItemPrice = vasdto.Price,
                    PriceType = PriceOfferType.Vas,
                    Quantity = vas.RequestMaxCount > 0 ? vas.RequestMaxCount : 1
                }) ;
            }
            return Task.FromResult(ShippingRequestVasesPricing);
        }



        #region Calculate
        /// <summary>
        /// Store the minumime commission value for main item
        /// </summary>
        private decimal _minValueCommission;
        /// <summary>
        /// Store the minumime commission value for vases
        /// </summary>
        private decimal _detailMinValueCommission;
        private void Calculate(PriceOffer offer, ShippingRequest shippingRequest, CreateOrEditPriceOfferInput input)
        {
            offer.TaxVat = _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);
            SetCalculateSettings(offer, shippingRequest,input);
            CalculatePriceOfferDetails(offer);
            CalculateSingle(offer);
            CalculateFinalPrices(offer);
            CalculateSingleWithCommission(offer);
            CalculateMultipleFinalPricesWithCommission(offer);
        }
        /// <summary>
        /// Manage to get all settings for start offer  calculate by settings
        /// </summary>
        /// <param name="offer"></param>
        /// <param name="shippingRequest"></param>
        /// <param name="input"></param>
        private void SetCalculateSettings(PriceOffer offer, ShippingRequest shippingRequest, CreateOrEditPriceOfferInput input)
        {
            switch (offer.PriceType)
            {
                case PriceOfferType.Trip:
                    SetCalculateTripSettings(offer, shippingRequest, input);
                    break;
            }

        }
        /// <summary>
        /// Set Commission Settings for shipment for Calculate
        /// </summary>
        /// <param name="offer"></param>
        /// <param name="shippingRequest"></param>
        private void SetCalculateTripSettings(PriceOffer offer, ShippingRequest shippingRequest,CreateOrEditPriceOfferInput input)
        {
            PriceOfferCommissionType vasCommissionType;
            decimal vasCommissionPercentageOrAddValue=0;
            offer.Quantity = shippingRequest.NumberOfTrips;
            if (_featureChecker.IsEnabled(AppFeatures.TachyonDealer))  {
                vasCommissionType = input.VasCommissionType.Value;
                vasCommissionPercentageOrAddValue = input.VasCommissionPercentageOrAddValue.Value;
            }
            else if (shippingRequest.IsTachyonDeal)
            {
                offer.CommissionType = (PriceOfferCommissionType)Convert.ToByte(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerTripCommissionType));
                _minValueCommission = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerTripMinValueCommission));
                vasCommissionType = (PriceOfferCommissionType)Convert.ToByte(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerVasCommissionType));
                _detailMinValueCommission = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerVasMinValueCommission));

                switch (offer.CommissionType)
                {
                    case PriceOfferCommissionType.CommissionPercentage:
                        offer.CommissionPercentageOrAddValue = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerTripCommissionPercentage));
                        vasCommissionPercentageOrAddValue = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerVasCommissionPercentage));
                        break;
                    case PriceOfferCommissionType.CommissionValue:
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
                    case PriceOfferCommissionType.CommissionPercentage:
                        offer.CommissionPercentageOrAddValue = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TripCommissionPercentage));
                        vasCommissionPercentageOrAddValue = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.VasCommissionPercentage));

                        break;
                    case PriceOfferCommissionType.CommissionValue:
                        offer.CommissionPercentageOrAddValue = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TripCommissionValue));
                        vasCommissionPercentageOrAddValue = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.VasCommissionValue));

                        break;
                }
            }

            SetPriceOfferDetailsCommissionSettings(offer, vasCommissionType, vasCommissionPercentageOrAddValue);
            CalculateCommission(offer);
        }


        /// <summary>
        /// Calculate TAD commission for main item
        /// </summary>
        /// <param name="offer"></param>
        private void CalculateCommission(PriceOffer offer)
        {
            switch (offer.CommissionType)
            {
                case PriceOfferCommissionType.CommissionPercentage:
                    offer.ItemCommissionAmount = (offer.ItemPrice * offer.CommissionPercentageOrAddValue / 100);
                    if (offer.ItemCommissionAmount < _minValueCommission && !_featureChecker.IsEnabled(AppFeatures.TachyonDealer))
                    {
                        offer.ItemCommissionAmount = _minValueCommission;
                        offer.CommissionType = PriceOfferCommissionType.CommissionMinimumValue ;
                    }
                    break;
                case PriceOfferCommissionType.CommissionValue:
                case PriceOfferCommissionType.CommissionMinimumValue:
                    offer.ItemCommissionAmount = offer.CommissionPercentageOrAddValue;
                    break;
            }
        }

        /// <summary>
        /// Calculate amount for each item
        /// </summary>
        /// <param name="offer"></param>
        private void CalculateSingle(PriceOffer offer)
        {
            offer.ItemVatAmount = TACHYON.Common.Calculate.CalculateVat(offer.ItemPrice, offer.TaxVat);
            offer.ItemTotalAmount = offer.ItemPrice + offer.ItemVatAmount;

        }
        /// <summary>
        /// Calculate totals for all items
        /// 
        /// </summary>
        /// <param name="offer"></param>
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
                case PriceOfferCommissionType.CommissionPercentage:
                    item.ItemCommissionAmount = (item.ItemPrice * item.CommissionPercentageOrAddValue / 100);

                    if (item.ItemCommissionAmount < _detailMinValueCommission && !_featureChecker.IsEnabled(AppFeatures.TachyonDealer))
                    {
                        item.ItemCommissionAmount = _detailMinValueCommission;
                        item.CommissionType = PriceOfferCommissionType.CommissionMinimumValue;
                    }
                    break;
                case PriceOfferCommissionType.CommissionValue:
                case PriceOfferCommissionType.CommissionMinimumValue:
                    item.ItemCommissionAmount =  item.CommissionPercentageOrAddValue;
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
