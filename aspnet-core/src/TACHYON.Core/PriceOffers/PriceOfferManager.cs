using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.EntityHistory;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using NUglify.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Configuration;
using TACHYON.EntityLogs.Transactions;
using TACHYON.Features;
using TACHYON.Invoices.Balances;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;
using TACHYON.PriceOffers.Dto;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PriceOffers
{
    public class PriceOfferManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<PriceOffer, long> _priceOfferRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestsRepository;
        private readonly IRepository<ShippingRequestDirectRequest, long> _shippingRequestDirectRequestRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly ISettingManager _settingManager;
        private readonly IFeatureChecker _featureChecker;
        private readonly IAbpSession _abpSession;
        private readonly BalanceManager _balanceManager;
        private ShippingRequestDirectRequest _directRequest;
        private readonly TenantManager _tenantManager;
        private readonly IEntityChangeSetReasonProvider _reasonProvider;

        public PriceOfferManager(IAppNotifier appNotifier, ISettingManager settingManager, IFeatureChecker featureChecker, IRepository<ShippingRequest, long> shippingRequestsRepository, IAbpSession abpSession, BalanceManager balanceManager, IRepository<ShippingRequestDirectRequest, long> shippingRequestDirectRequestRepository, IRepository<PriceOffer, long> priceOfferRepository, TenantManager tenantManager, IEntityChangeSetReasonProvider reasonProvider)
        {
            _appNotifier = appNotifier;
            _settingManager = settingManager;
            _featureChecker = featureChecker;
            _shippingRequestsRepository = shippingRequestsRepository;
            _abpSession = abpSession;
            _balanceManager = balanceManager;
            _shippingRequestDirectRequestRepository = shippingRequestDirectRequestRepository;
            _priceOfferRepository = priceOfferRepository;
            _tenantManager = tenantManager;
            _reasonProvider = reasonProvider;
        }

        /// <summary>
        /// Create Or Edit Offer
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<long> CreateOrEdit(CreateOrEditPriceOfferInput input)
        {
            DisableTenancyFilters();


            var shippingRequest = await _shippingRequestsRepository
                                        .GetAll()
                                        .Include(x => x.ShippingRequestVases)
                                        .FirstOrDefaultAsync(r => r.Id == input.ShippingRequestId);

            if (!shippingRequest.Status.IsIn(ShippingRequestStatus.NeedsAction, ShippingRequestStatus.PrePrice, ShippingRequestStatus.AcceptedAndWaitingCarrier))
            {
                throw new UserFriendlyException(L("TheShippingRequestNotFound"));
            }


            if (await IsTachyonDealer() && shippingRequest.IsTachyonDeal)
            {
                input.Channel = PriceOfferChannel.TachyonManageService;
            }

            if (input.Channel == PriceOfferChannel.MarketPlace)
            {
                MarketPlaceCanAccess(input, shippingRequest);
            }

            if (await IsTachyonDealer())
            {
                input.Channel = PriceOfferChannel.TachyonManageService;
            }

            if (input.Channel == PriceOfferChannel.DirectRequest)
            {
                await DirectRequestCanAccess(input, shippingRequest);
            }

            var offer = GetCarrierPricingOrNull(input.ShippingRequestId);

            if (offer == null || offer.Status == PriceOfferStatus.Rejected)
            {
                return await Create(input, shippingRequest, offer);
            }
            else
            {
                return await Update(input, shippingRequest, offer);
            }
        }

        public async Task<PriceOffer> InitPriceOffer(CreateOrEditPriceOfferInput input)
        {
            DisableTenancyFilters();
            using (CurrentUnitOfWork.DisableFilter("IHasIsDrafted")) // filter disabled for carrier as saas 
            {
                var shippingRequest = await _shippingRequestsRepository
                    .GetAll()
                    .Include(x => x.Tenant)
                    .Include(x => x.ShippingRequestVases)
                    .FirstOrDefaultAsync
                    (
                        r => r.Id == input.ShippingRequestId && (r.Status == ShippingRequestStatus.NeedsAction || r.Status == ShippingRequestStatus.PrePrice ||
                                                                 r.Status == ShippingRequestStatus.AcceptedAndWaitingCarrier)
                    );
                if (shippingRequest == null) throw new UserFriendlyException(L("TheShippingRequestNotFound"));

                if (await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer))
                    input.Channel = PriceOfferChannel.TachyonManageService;

                var offer = ObjectMapper.Map<PriceOffer>(input);
                offer.PriceOfferDetails = await GetListOfVases(input, shippingRequest);
                offer.ShippingRequestFk = shippingRequest;
                Calculate(offer, shippingRequest, input);

                return offer;
            }

        }


        /// <summary>
        /// Delete the offer when status the shipping request is needs action
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Delete(EntityDto<long> input)
        {
            DisableTenancyFilters();
            var pricing = await _priceOfferRepository
                .GetAll()
                .Include(x => x.ShippingRequestFk)
                .FirstOrDefaultAsync(x =>
                    x.Id == input.Id && (x.TenantId == _abpSession.TenantId.Value || !_abpSession.TenantId.HasValue) &&
                    (x.ShippingRequestFk.Status == ShippingRequestStatus.NeedsAction || x.ShippingRequestFk.Status ==
                        ShippingRequestStatus.AcceptedAndWaitingCarrier));
            if (pricing == null) throw new UserFriendlyException(L("TheRecordNotFound"));
            var request = pricing.ShippingRequestFk;
            if (pricing.Channel == PriceOfferChannel.MarketPlace)
            {
                if (pricing.ShippingRequestFk.BidStatus != ShippingRequestBidStatus.OnGoing)
                    throw new UserFriendlyException(L("TheRecordNotFound"));
                request.TotalOffers -= 1;
            }
            else if (pricing.Channel == PriceOfferChannel.DirectRequest)
            {
                var directRequest =
                    await _shippingRequestDirectRequestRepository.FirstOrDefaultAsync(x => x.Id == pricing.SourceId);
                if (directRequest != null) directRequest.Status = ShippingRequestDirectRequestStatus.New;
            }

            if (!await _priceOfferRepository.GetAll()
                    .AnyAsync(x => x.ShippingRequestId == request.Id && x.Id != input.Id))
            {
                request.Status = ShippingRequestStatus.PrePrice;
            }

            await _priceOfferRepository.DeleteAsync(pricing);
        }


        //Shipper Or TAD accept carrier offer Accept Offer
        public async Task<PriceOfferStatus> AcceptOffer(long id)
        {
            DisableTenancyFilters();
            var offer = await GetOffer(id);
            var canAcceptOrRejectOffer = await CanAcceptOrRejectOffer(offer);
            if (!canAcceptOrRejectOffer) throw new UserFriendlyException(L("YouCanNotAcceptTheOffer"));

            return await _AcceptOffer(offer);
        }

        private async Task<PriceOfferStatus> _AcceptOffer(PriceOffer offer)
        {
            _reasonProvider.Use(nameof(AcceptShippingRequestPriceOfferTransaction));

            await CheckIfThereOfferAcceptedBefore(offer.ShippingRequestId);
            if (!_abpSession.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                await TachyonAcceptOffer(offer);
                return offer.Status;
            }

            var request = offer.ShippingRequestFk;

            PriceOffer parentOffer = default;
            //Check if shipper have enough balance to pay 
            await _balanceManager.ShipperCanAcceptOffer(offer);

            /// Check if the offer send by TAD on market place
            if (offer.Tenant.EditionId == TachyonEditionId && !offer.ShippingRequestFk.IsTachyonDeal)
            {
                offer.Status = PriceOfferStatus.AcceptedAndWaitingForCarrier;
                request.IsBid = false;
                request.BidStatus = ShippingRequestBidStatus.StandBy;
                request.BidStartDate = default;
                request.BidEndDate = default;
                request.IsTachyonDeal = true;
                request.RequestType = ShippingRequestType.TachyonManageService;
            }
            /// Check if offer has carrier from parent offer coming
            else if (offer.ParentId.HasValue || !offer.ShippingRequestFk.IsTachyonDeal)
            {
                offer.Status = PriceOfferStatus.Accepted;
                request.Status = ShippingRequestStatus.PostPrice;
                if (offer.ParentId.HasValue)
                {
                    parentOffer = await GetOfferById(offer.ParentId.Value);
                }

                request.CarrierTenantId = parentOffer?.TenantId;
                if (request.CarrierTenantId == null)
                {
                    request.CarrierTenantId = offer.TenantId;
                }

                if (request.IsBid)
                {
                    request.BidStatus = ShippingRequestBidStatus.Closed;
                }

                if (parentOffer != null && parentOffer.Channel == PriceOfferChannel.DirectRequest)
                    await ChangeDirectRequestStatus(parentOffer.SourceId.Value,
                        ShippingRequestDirectRequestStatus.Accepted);
                if (offer.Channel == PriceOfferChannel.DirectRequest)
                    await ChangeDirectRequestStatus(offer.SourceId.Value, ShippingRequestDirectRequestStatus.Accepted);

                if (parentOffer != null) await _appNotifier.TMSAcceptedOffer(parentOffer);
            }
            else //TAD still need to find carrier to assign to shipping request
            {
                offer.Status = PriceOfferStatus.AcceptedAndWaitingForCarrier;
                //request.Status = ShippingRequestStatus.AcceptedAndWaitingCarrier;
            }

            SetShippingRequestPricing(offer);
            await _appNotifier.ShipperAcceptedOffer(offer);

            return offer.Status;
        }

        public async Task<PriceOfferStatus> AcceptOfferOnBehalfShipper(long id)
        {

            DisableTenancyFilters();
            var offer = await GetOffer(id);

            var canAcceptOrRejectOffer = await canAcceptOrRejectOfferOnBehalf(offer);
            if (!canAcceptOrRejectOffer) throw new UserFriendlyException(L("YouCanNotAcceptTheOffer"));
            var userId = _abpSession.UserId;
            using (_abpSession.Use(offer.ShippingRequestFk.TenantId,userId))
            {
                return await _AcceptOffer(offer);
            }


        }

        /// <summary>
        /// If the offer managed by TMS
        /// </summary>
        /// <param name="offer"></param>
        /// <returns></returns>
        private async Task TachyonAcceptOffer(PriceOffer offer)
        {
            if (!_abpSession.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                var exisistOffer = await _priceOfferRepository
                    .GetAll()
                    .Where(x => x.ShippingRequestId == offer.ShippingRequestId &&
                                x.Status == PriceOfferStatus.AcceptedAndWaitingForCarrier)
                    .FirstOrDefaultAsync();
                if (exisistOffer != null) // If true then have exists offer approve by shipper
                {
                    CheckIfPriceLowerThanSendByTms(exisistOffer.TotalAmount, offer.TotalAmount);

                    exisistOffer.ParentId = offer.Id;
                    exisistOffer.Status = PriceOfferStatus.Accepted;
                    offer.Status = PriceOfferStatus.Accepted;
                    offer.ShippingRequestFk.Status = ShippingRequestStatus.PostPrice;
                    if (offer.Channel == PriceOfferChannel.DirectRequest)
                    {
                        await ChangeDirectRequestStatus(offer.SourceId.Value,
                            ShippingRequestDirectRequestStatus.Accepted);
                    }

                    offer.ShippingRequestFk.CarrierTenantId = offer.TenantId;
                    if (offer.ShippingRequestFk.IsBid)
                        offer.ShippingRequestFk.BidStatus = ShippingRequestBidStatus.Closed;

                    await _appNotifier.TMSAcceptedOffer(offer);
                }
                else
                {
                    var tadOffer = await _priceOfferRepository
                        .GetAll()
                        .Where(x => x.ShippingRequestId == offer.ShippingRequestId &&
                                    x.Tenant.EditionId == TachyonEditionId &&
                                    (x.Status == PriceOfferStatus.New || x.Status == PriceOfferStatus.Rejected))
                        .FirstOrDefaultAsync();


                    if (tadOffer != null) /// If TMS send offer before accept carrier price then related offers togother
                    {
                        CheckIfPriceLowerThanSendByTms(tadOffer.TotalAmount, offer.TotalAmount);
                        tadOffer.ParentId = offer.Id;
                        tadOffer.Status = PriceOfferStatus.AcceptedAndWaitingForShipper;
                    }

                    offer.Status = PriceOfferStatus.Pending;
                    if (offer.Channel == PriceOfferChannel.DirectRequest)
                    {
                        await ChangeDirectRequestStatus(offer.SourceId.Value,
                            ShippingRequestDirectRequestStatus.Pending);
                    }

                    await _appNotifier.PendingOffer(offer);
                }
            }
        }

        /// <summary>
        /// If the TMS send price before to shipper then check if the price lower than carrier accepted 
        /// </summary>
        /// <param name="tmsTotalAmount"></param>
        /// <param name="carrierTotalAmount"></param>
        private void CheckIfPriceLowerThanSendByTms(decimal tmsTotalAmount, decimal carrierTotalAmount)
        {
            if (tmsTotalAmount < carrierTotalAmount)
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

            var offer = await GetOffer(input.Id);
            var canAcceptOrRejectOffer = await CanAcceptOrRejectOffer(offer);
            if (!canAcceptOrRejectOffer) throw new UserFriendlyException(L("YouCanNotRejectTheOffer"));
            await CheckIfThereOfferAcceptedBefore(offer.ShippingRequestId);
            offer.Status = PriceOfferStatus.Rejected;
            offer.RejectedReason = input.Reason;
            if (offer.Channel == PriceOfferChannel.DirectRequest)
            {
                var directRequest =
                    await _shippingRequestDirectRequestRepository.FirstOrDefaultAsync(x => x.Id == offer.SourceId);
                if (directRequest != null) directRequest.Status = ShippingRequestDirectRequestStatus.Rejected;
            }

            await _appNotifier.RejectedOffer(offer, input.RejectBy);
        }

        private async Task<PriceOffer> GetOffer(long id)
        {
            return await _priceOfferRepository.GetAll()
                   .Include(r => r.ShippingRequestFk).ThenInclude(r => r.Tenant).Include(t => t.Tenant)
                   .Where(x => x.Id == id)
                   .FirstOrDefaultAsync();
        }

        public async Task<bool> CanAcceptOrRejectOffer(PriceOffer offer)
        {

            //SR status check
            if (!offer.ShippingRequestFk.Status.IsIn(ShippingRequestStatus.NeedsAction, ShippingRequestStatus.AcceptedAndWaitingCarrier))
            {
                return false;
            }

            //Shipper 
            if (await _featureChecker.IsEnabledAsync(AppFeatures.Shipper))
            {
                //offer status
                var isAllowedStatus = offer.Status.IsIn(PriceOfferStatus.New, PriceOfferStatus.AcceptedAndWaitingForShipper);
                if (isAllowedStatus && offer.ShippingRequestFk.TenantId == _abpSession.TenantId && (!offer.ShippingRequestFk.IsTachyonDeal || offer.Channel == PriceOfferChannel.TachyonManageService))
                {
                    return true;
                }


            }
            //TMS or host
            if (!_abpSession.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                if (offer.ShippingRequestFk.IsTachyonDeal && offer.Tenant.EditionId != 4 && offer.Status == PriceOfferStatus.New)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> canAcceptOrRejectOfferOnBehalf(PriceOffer offer)
        {

            //SR status check
            if (!offer.Status.IsIn(PriceOfferStatus.AcceptedAndWaitingForShipper))
            {
                return false;
            }

            //TMS or host
            if (await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer))
            {

                return true;

            }

            return false;
        }


        public bool CanEditOffer(PriceOffer offer)
        {

            if (offer.ShippingRequestFk.Status == ShippingRequestStatus.NeedsAction) // SR pre-price
            {
                if (offer.Status.IsIn(PriceOfferStatus.New, PriceOfferStatus.Rejected)) // offer not accepted 
                {
                    if (offer.TenantId == _abpSession.TenantId) // my offers only 
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private async Task CheckIfThereOfferAcceptedBefore(long id)
        {
            var offer = await _priceOfferRepository
                .GetAll()
                .Include(r => r.ShippingRequestFk)
                .Where(x => x.ShippingRequestId == id)
                .WhereIf(_abpSession.TenantId.HasValue && await _featureChecker.IsEnabledAsync(AppFeatures.Shipper),
                    x => x.ShippingRequestFk.TenantId == _abpSession.TenantId &&
                         x.Status == PriceOfferStatus.AcceptedAndWaitingForCarrier)
                .WhereIf(
                    !_abpSession.TenantId.HasValue || await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer),
                    x => x.ShippingRequestFk.IsTachyonDeal &&
                         (x.Status == PriceOfferStatus.AcceptedAndWaitingForShipper ||
                          x.Status == PriceOfferStatus.Pending))
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
        public async Task<PriceOffer> GetOfferAcceptedByShippingRequestId(long id)
        {
            return (await _priceOfferRepository
                .GetAllIncluding(x => x.PriceOfferDetails)
                .FirstOrDefaultAsync(x => x.ShippingRequestId == id && x.Status == PriceOfferStatus.Accepted));
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
            return (await _priceOfferRepository.GetAll().OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(x => x.SourceId == sourceId && x.Channel == channel));
        }

        #region Helper

        /// <summary>
        /// Load offer pricing approve by shipper in shipment
        /// </summary>
        /// <param name="offer"></param>
        /// <returns></returns>
        public void SetShippingRequestPricing(PriceOffer offer)
        {
            var request = offer.ShippingRequestFk;
            request.Price = offer.TotalAmountWithCommission;
            request.SubTotalAmount = offer.SubTotalAmountWithCommission;
            request.VatAmount = offer.VatAmountWithCommission;
            request.VatSetting = offer.TaxVat;
            request.TotalCommission = offer.TotalAmountWithCommission;
            request.CarrierPrice = offer.TotalAmount;

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
                .OrderByDescending(x => x.Id)
                .FirstOrDefault(x =>
                    x.ShippingRequestId == requestId && x.TenantId == _abpSession.TenantId.Value &&
                    x.Status != PriceOfferStatus.ForceRejected);
        }

        /// <summary>
        /// Check if the carrier already make pricing for the shipment or not
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public bool CheckCarrierIsPricing(long requestId)
        {
            return _priceOfferRepository.GetAll().Any(x =>
                x.ShippingRequestId == requestId && x.TenantId == _abpSession.TenantId.Value);
        }

        /// <summary>
        /// Get the total offers add by TMS when shipper view the shipping list
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public int GetTotalOffersByTms(long requestId)
        {
            return _priceOfferRepository.GetAll()
                .Where(x => x.ShippingRequestId == requestId && x.Tenant.EditionId == TachyonEditionId).Count();
        }

        /// <summary>
        /// Check If the shipping request in market place to access to add offer
        /// </summary>
        /// <param name="input"></param>
        /// <param name="shippingRequest"></param>
        private void MarketPlaceCanAccess(CreateOrEditPriceOfferInput input, ShippingRequest shippingRequest)
        {
            if (shippingRequest.BidStatus != ShippingRequestBidStatus.OnGoing)
                throw new UserFriendlyException(L("The Bid must be Ongoing"));
            //if (_featureChecker.IsEnabled(AppFeatures.TachyonDealer) && shippingRequest.IsTachyonDeal) throw new UserFriendlyException(L("YouCanNotMakeBidFromMarketPlaceWhenTheShippingManageByTAD"));
        }

        /// <summary>
        /// Check the carrier of have direct request to access to add offer
        /// </summary>
        /// <param name="input"></param>
        /// <param name="shippingRequest"></param>
        /// <returns></returns>
        private async Task DirectRequestCanAccess(CreateOrEditPriceOfferInput input, ShippingRequest shippingRequest)
        {
            _directRequest = await _shippingRequestDirectRequestRepository.FirstOrDefaultAsync(x =>
                x.CarrierTenantId == _abpSession.TenantId.Value && x.ShippingRequestId == input.ShippingRequestId);
            if (_directRequest == null) throw new UserFriendlyException(L("YouDoNotHaveDirectRequest"));
            if (_directRequest.Status == ShippingRequestDirectRequestStatus.Accepted)
                throw new UserFriendlyException(L("YouCanNoCreateOrEditPriceWhenThePriceAccepted"));
            input.SourceId = _directRequest.Id;
        }

        /// <summary>
        /// Change the direct request status
        /// </summary>
        /// <param name="directRequestId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task ChangeDirectRequestStatus(long directRequestId, ShippingRequestDirectRequestStatus status)
        {
            _directRequest =
                await _shippingRequestDirectRequestRepository.FirstOrDefaultAsync(x => x.Id == directRequestId);
            _directRequest.Status = status;
        }

        /// <summary>
        /// Create offer
        /// </summary>
        /// <param name="input"></param>
        /// <param name="shippingRequest"></param>
        /// <param name="rejectedOffer"></param>
        /// <returns></returns>
        private async Task<long> Create(CreateOrEditPriceOfferInput input,
            ShippingRequest shippingRequest,
            PriceOffer rejectedOffer)
        {
            _reasonProvider.Use(nameof(CreateShippingRequestPriceOfferTransaction));
            var offer = ObjectMapper.Map<PriceOffer>(input);
            offer.PriceOfferDetails = await GetListOfVases(input, shippingRequest);
            offer.ShippingRequestFk = shippingRequest;
            Calculate(offer, shippingRequest, input);

            if (rejectedOffer !=
                null) /// If the user re priced the price from rejected offer then change the status for prevoius rejected to forcerejected
            {
                if (rejectedOffer.ParentId.HasValue)
                    offer.ParentId =
                        rejectedOffer
                            .ParentId; /// if the rejected related with parent offer set the parent offer for new offer created
                rejectedOffer.Status = PriceOfferStatus.ForceRejected;
            }

            offer.Id = await _priceOfferRepository.InsertAndGetIdAsync(offer);
            if (shippingRequest.Status != ShippingRequestStatus.NeedsAction)
                shippingRequest.Status = ShippingRequestStatus.NeedsAction;
            shippingRequest.TotalOffers += 1;
            if (offer.Channel == PriceOfferChannel.DirectRequest)
                _directRequest.Status = ShippingRequestDirectRequestStatus.Response;

            if (offer.ParentId != null) offer.Status = PriceOfferStatus.AcceptedAndWaitingForShipper;
            await _appNotifier.ShippingRequestSendOfferWhenAddPrice(offer, GetCurrentTenant(_abpSession).Name);

            await _appNotifier.NotifyShipperWhenSendPriceOffer(shippingRequest.TenantId, offer.Id);


            await CurrentUnitOfWork.SaveChangesAsync();
            return offer.Id;
        }

        /// <summary>
        /// Update offer
        /// </summary>
        /// <param name="input"></param>
        /// <param name="shippingRequest"></param>
        /// <param name="offer"></param>
        /// <returns></returns>
        private async Task<long> Update(CreateOrEditPriceOfferInput input,
            ShippingRequest shippingRequest,
            PriceOffer offer)
        {
            _reasonProvider.Use(nameof(UpdateShippingRequestPriceOfferTransaction));
            if (!CanEditOffer(offer))
            {
                throw new UserFriendlyException(L("YouCantEditOffer"));
            }

            if (offer.Status == PriceOfferStatus.Accepted) throw new UserFriendlyException(L("YourPriceAlreadyAcceptedYouCanEdit"));
            var channel = offer.Channel;
            ObjectMapper.Map(input, offer);
            offer.Channel = channel;
            offer.PriceOfferDetails.Clear();
            offer.PriceOfferDetails = await GetListOfVases(input, shippingRequest);
            Calculate(offer, shippingRequest, input);
            if (offer.IsView)
            {
                offer.IsView = false;
                offer.ShippingRequestFk = shippingRequest;
                await _appNotifier.ShippingRequestSendOfferWhenUpdatePrice(offer, GetCurrentTenant(_abpSession).Name);
            }

            await _priceOfferRepository.UpdateAsync(offer);

            await CurrentUnitOfWork.SaveChangesAsync();
            return offer.Id;
        }

        /// <summary>
        /// Get list of vases after merege data between shipemnt vases and input vases
        /// </summary>
        /// <param name="input"></param>
        /// <param name="shippingRequest"></param>
        /// <returns></returns>
        private Task<List<PriceOfferDetail>> GetListOfVases(CreateOrEditPriceOfferInput input,
            ShippingRequest shippingRequest)
        {
            List<PriceOfferDetail> shippingRequestVasesPricing = new List<PriceOfferDetail>();
            if (input.ItemDetails.Count != shippingRequest.ShippingRequestVases.Count)
            {
                throw new UserFriendlyException(L("YouShouldAddPricesForAllVases"));
            }

            if (input.ItemDetails.Any(x => x.Price < 0))
                throw new UserFriendlyException(L("ThePriceMustBeGreaterThanZero"));
            foreach (var vas in shippingRequest.ShippingRequestVases)
            {
                var vasDto = input.ItemDetails.FirstOrDefault(x => x.ItemId == vas.Id);
                if (vasDto == null) throw new UserFriendlyException(L("YouShouldAddVasRelatedWithShippingRequest"));

                shippingRequestVasesPricing.Add(new PriceOfferDetail()
                {
                    SourceId = vas.Id,
                    ItemPrice = vasDto.Price,
                    PriceType = PriceOfferType.Vas,
                    Quantity = vas.RequestMaxCount > 0 ? vas.RequestMaxCount : 1
                });
            }

            return Task.FromResult(shippingRequestVasesPricing);
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

        private void Calculate(PriceOffer offer,
            ShippingRequest shippingRequest,
            CreateOrEditPriceOfferInput input)
        {
            offer.TaxVat = _settingManager.GetSettingValue<decimal>(AppSettings.HostManagement.TaxVat);

            // set commissions defaults for trip and vases 
            SetCalculateSettings(offer, shippingRequest, input);

            // priceOffer items list calculation 
            CalculatePriceOfferDetails(offer);

            // ItemVatAmount , ItemTotalAmount
            CalculateSingle(offer);

            //SubTotalAmount, VatAmount, TotalAmount
            CalculateFinalPrices(offer);

            //ItemSubTotalAmountWithCommission, ItemVatAmountWithCommission, ItemTotalAmountWithCommission
            CalculateSingleWithCommission(offer);

            //SubTotalAmountWithCommission, VatAmountWithCommission, TotalAmountWithCommission, CommissionAmount
            CalculateMultipleFinalPricesWithCommission(offer);
        }


        /// <summary>
        /// Manage to get all settings for start offer  calculate by settings
        /// </summary>
        /// <param name="offer"></param>
        /// <param name="shippingRequest"></param>
        /// <param name="input"></param>
        private void SetCalculateSettings(PriceOffer offer,
            ShippingRequest shippingRequest,
            CreateOrEditPriceOfferInput input)
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
        /// <param name="input"></param>
        private void SetCalculateTripSettings(PriceOffer offer,
            ShippingRequest shippingRequest,
            CreateOrEditPriceOfferInput input)
        {
            #region settings

            // TMS
            var tachyonDealerTripCommissionType = (PriceOfferCommissionType)Convert.ToByte(
                _featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerTripCommissionType));
            var tachyonDealerTripMinValueCommission = Convert.ToDecimal(
                _featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerTripMinValueCommission));
            var tachyonDealerVasCommissionType = (PriceOfferCommissionType)Convert.ToByte(
                _featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerVasCommissionType));
            decimal tachyonDealerVasMinValueCommission = Convert.ToDecimal(
                _featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerVasMinValueCommission));
            decimal tachyonDealerTripCommissionPercentage = Convert.ToDecimal(
                _featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerTripCommissionPercentage));
            decimal tachyonDealerVasCommissionPercentage = Convert.ToDecimal(
                _featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerVasCommissionPercentage));
            decimal tachyonDealerTripCommissionValue = Convert.ToDecimal(
                _featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerTripCommissionValue));
            decimal tachyonDealerVasCommissionValue = Convert.ToDecimal(
                _featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerVasCommissionValue));
            // marketPlace
            var tripCommissionType =
                (PriceOfferCommissionType)Convert.ToByte(_featureChecker.GetValue(shippingRequest.TenantId,
                    AppFeatures.TripCommissionType));
            var tripMinValueCommission =
                Convert.ToDecimal(
                    _featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TripMinValueCommission));
            var vasCommissionType =
                (PriceOfferCommissionType)Convert.ToByte(_featureChecker.GetValue(shippingRequest.TenantId,
                    AppFeatures.VasCommissionType));
            var vasMinValueCommission =
                Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId,
                    AppFeatures.VasMinValueCommission));
            var tripCommissionPercentage =
                Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId,
                    AppFeatures.TripCommissionPercentage));
            var vasCommissionPercentage =
                Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId,
                    AppFeatures.VasCommissionPercentage));
            var tripCommissionValue =
                Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TripCommissionValue));
            var vasCommissionValue =
                Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.VasCommissionValue));
            // direct request
            try { var directRequestCommissionTypes = (PriceOfferCommissionType)Convert.ToByte(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.DirectRequestCommissionType)); }
            catch (Exception) { throw new UserFriendlyException(L("ShipperDoseNotHavedirectRequestCommissionFeautre", shippingRequest.Tenant.TenancyName)); }

            var directRequestCommissionType = (PriceOfferCommissionType)Convert.ToByte(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.DirectRequestCommissionType));
            var directRequestCommissionMinValue = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.DirectRequestCommissionMinValue));
            var directRequestVasCommissionMinValue = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.DirectRequestVasCommissionMinValue));
            var directRequestCommissionPercentage = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.DirectRequestCommissionPercentage));
            var directRequestVasCommissionPercentage = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.DirectRequestVasCommissionPercentage));
            var directRequestCommissionValue = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.DirectRequestCommissionValue));
            var directRequestVasCommissionValue = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.DirectRequestVasCommissionValue));
            if (offer.Channel == PriceOfferChannel.CarrierAsSaas)
            {
                decimal carrierAsSaasCommissionValue = Convert.ToDecimal(_featureChecker.GetValue(shippingRequest.TenantId, AppFeatures.CarrierAsSaasCommissionValue));

                directRequestCommissionType = PriceOfferCommissionType.CommissionValue;
                directRequestCommissionMinValue = carrierAsSaasCommissionValue;
                directRequestVasCommissionMinValue = carrierAsSaasCommissionValue;
                directRequestCommissionPercentage = 0;
                directRequestVasCommissionPercentage = 0;
                directRequestCommissionValue = carrierAsSaasCommissionValue;
                directRequestVasCommissionValue = 0;

            }

            #endregion


            decimal vasCommissionPercentageOrAddValue = 0;
            offer.Quantity = shippingRequest.NumberOfTrips;

            // TMS user can overwrite default commissions settings 
            if (_featureChecker.IsEnabled(AppFeatures.TachyonDealer))
            {
                if (input.VasCommissionType != null)
                {
                    vasCommissionType = input.VasCommissionType.Value;
                }

                if (input.VasCommissionPercentageOrAddValue != null)
                {
                    vasCommissionPercentageOrAddValue = input.VasCommissionPercentageOrAddValue.Value;
                }
            }
            // set default commission settings for TMS request 
            else if (shippingRequest.IsTachyonDeal)
            {
                // TMS trip default  commission type
                offer.CommissionType = tachyonDealerTripCommissionType;
                _minValueCommission = tachyonDealerTripMinValueCommission;

                // TMS VAS default commission type 
                vasCommissionType = tachyonDealerVasCommissionType;
                _detailMinValueCommission = tachyonDealerVasMinValueCommission;

                // set default TMS commission values 
                switch (offer.CommissionType)
                {
                    case PriceOfferCommissionType.CommissionPercentage:
                        offer.CommissionPercentageOrAddValue = tachyonDealerTripCommissionPercentage;
                        vasCommissionPercentageOrAddValue = tachyonDealerVasCommissionPercentage;
                        break;
                    case PriceOfferCommissionType.CommissionValue:
                        offer.CommissionPercentageOrAddValue = tachyonDealerTripCommissionValue;
                        vasCommissionPercentageOrAddValue = tachyonDealerVasCommissionValue;

                        break;
                }
            }
            // set default commission settings for marketPlace
            else if (shippingRequest.IsBid)
            {
                offer.CommissionType = tripCommissionType;
                _minValueCommission = tripMinValueCommission;
                _detailMinValueCommission = vasMinValueCommission;
                switch (offer.CommissionType)
                {
                    case PriceOfferCommissionType.CommissionPercentage:
                        offer.CommissionPercentageOrAddValue = tripCommissionPercentage;
                        vasCommissionPercentageOrAddValue = vasCommissionPercentage;
                        break;
                    case PriceOfferCommissionType.CommissionValue:
                        offer.CommissionPercentageOrAddValue = tripCommissionValue;
                        vasCommissionPercentageOrAddValue = vasCommissionValue;
                        break;
                }
            }
            else if (shippingRequest.IsDirectRequest)
            {
                offer.CommissionType = directRequestCommissionType;
                _minValueCommission = directRequestCommissionMinValue;
                _detailMinValueCommission = directRequestVasCommissionMinValue;
                switch (offer.CommissionType)
                {
                    case PriceOfferCommissionType.CommissionPercentage:
                        offer.CommissionPercentageOrAddValue = directRequestCommissionPercentage;
                        vasCommissionPercentageOrAddValue = directRequestVasCommissionPercentage;
                        break;
                    case PriceOfferCommissionType.CommissionValue:
                        offer.CommissionPercentageOrAddValue = directRequestCommissionValue;
                        vasCommissionPercentageOrAddValue = directRequestVasCommissionValue;
                        break;
                }
            }

            //set commission  for VASes
            SetPriceOfferDetailsCommissionSettings(offer, vasCommissionType, vasCommissionPercentageOrAddValue);
            // set commission  for trips
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
                    // set mini commission value
                    if (offer.ItemCommissionAmount < _minValueCommission &&
                        !_featureChecker.IsEnabled(AppFeatures.TachyonDealer))
                    {
                        offer.ItemCommissionAmount = _minValueCommission;
                        offer.CommissionType = PriceOfferCommissionType.CommissionMinimumValue;
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
            offer.SubTotalAmount =
                (offer.ItemPrice * offer.Quantity) + offer.PriceOfferDetails.Sum(x => x.SubTotalAmount);
            offer.VatAmount = (offer.ItemVatAmount * offer.Quantity) + offer.PriceOfferDetails.Sum(x => x.VatAmount);
            offer.TotalAmount = (offer.ItemTotalAmount * offer.Quantity) +
                                offer.PriceOfferDetails.Sum(x => x.TotalAmount);
            offer.ItemsTotalPricePreCommissionPreVat = offer.ItemPrice * offer.Quantity;
            offer.ItemsTotalVatAmountPreCommission =
                Common.Calculate.CalculateVat(offer.ItemsTotalPricePreCommissionPreVat, offer.TaxVat);
            offer.DetailsTotalPricePreCommissionPreVat = offer.PriceOfferDetails.Sum(x => x.ItemPrice * x.Quantity);
            offer.DetailsTotalVatAmountPreCommission =
                Common.Calculate.CalculateVat(offer.DetailsTotalPricePreCommissionPreVat, offer.TaxVat);
        }

        private void CalculateSingleWithCommission(PriceOffer offer)
        {
            offer.ItemSubTotalAmountWithCommission = offer.ItemPrice + offer.ItemCommissionAmount;
            offer.ItemVatAmountWithCommission =
                TACHYON.Common.Calculate.CalculateVat(offer.ItemSubTotalAmountWithCommission, offer.TaxVat);
            offer.ItemTotalAmountWithCommission =
                offer.ItemSubTotalAmountWithCommission + offer.ItemVatAmountWithCommission;
        }

        private void CalculateMultipleFinalPricesWithCommission(PriceOffer offer)
        {
            offer.SubTotalAmountWithCommission = (offer.ItemSubTotalAmountWithCommission * offer.Quantity) +
                                                 offer.PriceOfferDetails.Sum(x => x.SubTotalAmountWithCommission);
            offer.VatAmountWithCommission = (offer.ItemVatAmountWithCommission * offer.Quantity) +
                                            offer.PriceOfferDetails.Sum(x => x.VatAmountWithCommission);
            offer.TotalAmountWithCommission = (offer.ItemTotalAmountWithCommission * offer.Quantity) +
                                              offer.PriceOfferDetails.Sum(x => x.TotalAmountWithCommission);
            offer.CommissionAmount = (offer.ItemCommissionAmount * offer.Quantity) +
                                     offer.PriceOfferDetails.Sum(x => x.CommissionAmount);
            offer.ItemsTotalCommission = offer.ItemCommissionAmount * offer.Quantity;
            offer.DetailsTotalCommission = offer.PriceOfferDetails.Sum(x => x.CommissionAmount);
            offer.ItemsTotalPricePostCommissionPreVat =
                offer.ItemsTotalPricePreCommissionPreVat + offer.ItemsTotalCommission;
            offer.DetailsTotalPricePostCommissionPreVat =
                offer.DetailsTotalPricePreCommissionPreVat + offer.DetailsTotalCommission;
            offer.ItemsTotalVatPostCommission =
                Common.Calculate.CalculateVat(offer.ItemsTotalPricePostCommissionPreVat, offer.TaxVat);
            offer.DetailsTotalVatPostCommission =
                Common.Calculate.CalculateVat(offer.DetailsTotalPricePostCommissionPreVat, offer.TaxVat);
        }

        #region Details

        private void SetPriceOfferDetailsCommissionSettings(PriceOffer offer,
            PriceOfferCommissionType commissionType,
            decimal commissionPercentageOrAddValue)
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
                // ItemCommissionAmount 
                CalculateDetailCommission(item);
                // ItemVatAmount, ItemTotalAmount
                CalculateSingleDetail(offer, item);
                // SubTotalAmount, TotalAmount,VatAmount
                CalculateMultipleDetails(item);
                // ItemSubTotalAmountWithCommission, ItemVatAmountWithCommission, ItemTotalAmountWithCommission
                CalculateSingleDetailWithCommission(offer, item);
                // SubTotalAmountWithCommission, VatAmountWithCommission, TotalAmountWithCommission, CommissionAmount
                CalculateMultipleDetailsWithCommission(item);
            }
        }

        private void CalculateDetailCommission(PriceOfferDetail item)
        {
            switch (item.CommissionType)
            {
                case PriceOfferCommissionType.CommissionPercentage:
                    item.ItemCommissionAmount = (item.ItemPrice * item.CommissionPercentageOrAddValue / 100);

                    if (item.ItemCommissionAmount < _detailMinValueCommission &&
                        !_featureChecker.IsEnabled(AppFeatures.TachyonDealer))
                    {
                        item.ItemCommissionAmount = _detailMinValueCommission;
                        item.CommissionType = PriceOfferCommissionType.CommissionMinimumValue;
                    }

                    break;
                case PriceOfferCommissionType.CommissionValue:
                case PriceOfferCommissionType.CommissionMinimumValue:
                    item.ItemCommissionAmount = item.CommissionPercentageOrAddValue;
                    break;
            }
        }

        private void CalculateSingleDetail(PriceOffer offer, PriceOfferDetail item)
        {
            item.ItemVatAmount = TACHYON.Common.Calculate.CalculateVat(item.ItemPrice, offer.TaxVat);
            item.ItemTotalAmount = item.ItemPrice + item.ItemVatAmount;
            item.ItemsTotalPricePreCommissionPreVat = item.ItemPrice * item.Quantity;
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
            item.ItemVatAmountWithCommission =
                TACHYON.Common.Calculate.CalculateVat(item.ItemSubTotalAmountWithCommission, offer.TaxVat);
            item.ItemTotalAmountWithCommission =
                item.ItemSubTotalAmountWithCommission + item.ItemVatAmountWithCommission;
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

        protected virtual Tenant GetCurrentTenant(IAbpSession abpSession)
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                return _tenantManager.GetById(abpSession.GetTenantId());
            }
        }

        protected async Task<bool> IsCarrier()
        {
            return await _featureChecker.IsEnabledAsync(AppFeatures.Carrier);
        }
        protected async Task<bool> IsShipper()
        {
            return await _featureChecker.IsEnabledAsync(AppFeatures.Shipper);
        }

        protected async Task<bool> IsTachyonDealer()
        {
            return await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer);
        }
        #endregion
    }
}