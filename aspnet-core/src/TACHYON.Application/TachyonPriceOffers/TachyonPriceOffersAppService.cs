using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.Invoices.Balances;
using TACHYON.Notifications;
using TACHYON.Shipping.ShippingRequestBids;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.TachyonDealer;
using TACHYON.TachyonPriceOffers.dtos;

namespace TACHYON.TachyonPriceOffers
{
    public class TachyonPriceOffersAppService : TACHYONAppServiceBase, ITachyonPriceOffersAppService
    {

        private readonly IRepository<TachyonPriceOffer> _tachyonPriceOfferRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly CommissionManager _commissionManager;
        private readonly BalanceManager _balanceManager;
        private readonly ShippingRequestManager _shippingRequestManager;
        private readonly IRepository<ShippingRequestBid, long> _shippingRequestBidsRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly IRepository<ShippingRequestsCarrierDirectPricing> _shippingRequestsCarrierDirectPricingRepository;
        private readonly IRepository<ShippingRequestBid, long> _shippingRequestBidRepository;
        private readonly ShippingRequestBidManager _shippingRequestBidManager;


        public TachyonPriceOffersAppService(IRepository<TachyonPriceOffer> tachyonPriceOfferRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository, CommissionManager commissionManager, BalanceManager balanceManager,
            IRepository<ShippingRequestsCarrierDirectPricing> shippingRequestsCarrierDirectPricingRepository,
            ShippingRequestManager shippingRequestManager,
            IRepository<ShippingRequestBid, long> shippingRequestBidsRepository,
            IAppNotifier appNotifier, ShippingRequestBidManager shippingRequestBidManager)
        {
            _tachyonPriceOfferRepository = tachyonPriceOfferRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _commissionManager = commissionManager;
            _balanceManager = balanceManager;
            _shippingRequestManager = shippingRequestManager;
            _shippingRequestBidsRepository = shippingRequestBidsRepository;
            _appNotifier = appNotifier;
            _shippingRequestsCarrierDirectPricingRepository = shippingRequestsCarrierDirectPricingRepository;
            _shippingRequestBidRepository = shippingRequestBidsRepository;
            _shippingRequestBidManager = shippingRequestBidManager;
        }
        [RequiresFeature(AppFeatures.TachyonDealer, AppFeatures.Shipper)]
        public async Task<PagedResultDto<GetAllTachyonPriceOfferOutput>> GetAllTachyonPriceOffers(GetAllTachyonPriceOfferInput input)
        {
            DisableTenancyFilters();
            var filteredOffers = _tachyonPriceOfferRepository.GetAll()
                .Include(e => e.ShippingRequestFk)
                .WhereIf(await IsEnabledAsync(AppFeatures.TachyonDealer), e => e.TenantId == AbpSession.TenantId)
                .WhereIf(await IsEnabledAsync(AppFeatures.Shipper), e => e.ShippingRequestFk.TenantId == AbpSession.TenantId)
                .Where(e => e.ShippingRequestId == input.ShippingRequestId);


            var pagedAndFilteredOffers = filteredOffers
                .OrderBy(input.Sorting ?? "id Desc")
                .PageBy(input);

            var totalCount = await pagedAndFilteredOffers.CountAsync();

            var output = pagedAndFilteredOffers.Select(x =>
                          new GetAllTachyonPriceOfferOutput()
                          {
                              tachyonPriceOfferDto = ObjectMapper.Map<TachyonPriceOfferDto>(x)
                          });

            return new PagedResultDto<GetAllTachyonPriceOfferOutput>(totalCount, await output.ToListAsync());
        }

        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task CreateOrEditTachyonPriceOffer(CreateOrEditTachyonPriceOfferDto input)
        {
            DisableTenancyFilters();
            var Request = await GetShippingRequestOnPrePriceStage(input.ShippingRequestId);
            var Offer = await _tachyonPriceOfferRepository
                .FirstOrDefaultAsync(x => x.ShippingRequestId == input.ShippingRequestId &&
            x.OfferStatus == OfferStatus.AcceptedAndWaitingForCarrier);

            if (Offer == null)
            {
                Request.Status = ShippingRequestStatus.NeedsAction;
                await Create(input, Request);
            }
            else
            {
                if (!input.ShippingRequestBidId.HasValue && !input.DriectRequestForCarrierId.HasValue) throw new UserFriendlyException(L("ItShouldHaveCarrirer"));
                await Edit(input, Offer, Request);
            }
        }



        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task<GetTachyonPriceOfferForViewOutput> GetTachyonPriceOfferForView(EntityDto entity)
        {
            var item = await GetTachyonPriceOffer(entity.Id);
            return new GetTachyonPriceOfferForViewOutput
            {
                tachyonPriceOfferDto = ObjectMapper.Map<TachyonPriceOfferDto>(item)
            };
        }

        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task Delete(EntityDto entity)
        {
            var offer = await _tachyonPriceOfferRepository.FirstOrDefaultAsync(x => x.Id == entity.Id && x.OfferStatus == OfferStatus.Pending);
            if (offer == null)
            {
                throw new UserFriendlyException(L("Cannot delete accepted offer message"));
            }
            await _tachyonPriceOfferRepository.DeleteAsync(offer);
        }

        [RequiresFeature(AppFeatures.Shipper)]
        public async Task AcceptOrRejectOfferByShipper(AcceptOrRejectOfferByShipperInput input)
        {
            DisableTenancyFilters();
            var offer = await _tachyonPriceOfferRepository
                .GetAll()
                .Include(x => x.ShippingRequestFk)
                 .ThenInclude(x => x.Tenant)
                .FirstOrDefaultAsync(x => x.Id == input.Id &&
                x.OfferStatus == OfferStatus.Pending &&
                x.ShippingRequestFk.TenantId == AbpSession.TenantId);
            if (offer == null) throw new UserFriendlyException(L("Offer should be in pending status"));
            if (input.IsAccepted)
            {
                if (!offer.CarrirerTenantId.HasValue)
                {
                    offer.OfferStatus = OfferStatus.AcceptedAndWaitingForCarrier;
                }
                else
                {
                    //if bidding, set bid as accepted, 
                    await ChangeBiddingOrDirectRequestStatus(offer);

                    await _balanceManager.ShipperCanAcceptPrice(offer.ShippingRequestFk.TenantId, offer.TotalAmount, offer.ShippingRequestId);
                    // ObjectMapper.Map(offer, offer.ShippingRequestFk);
                    offer.ShippingRequestFk.CarrierTenantId = offer.CarrirerTenantId;
                    offer.ShippingRequestFk.Price = offer.TotalAmount;
                    await _shippingRequestManager.SetToPostPrice(offer.ShippingRequestFk);
                    offer.OfferStatus = OfferStatus.Accepted;

                }
                await _appNotifier.TachyonDealOfferAccepByShipper(offer);

            }
            else
            {
                offer.OfferStatus = OfferStatus.Rejected;
                offer.RejectedReason = input.RejectedReason;
                offer.ShippingRequestFk.Status = ShippingRequestStatus.PrePrice;
                await _appNotifier.TachyonDealOfferRejectedByShipper(offer);
            }

        }

        private async Task ChangeBiddingOrDirectRequestStatus(TachyonPriceOffer offer)
        {
            if (offer.ShippingRequestBidId != null)
            {
                var bid = await _shippingRequestBidRepository.GetAll()
                    .Include(x => x.ShippingRequestFk).FirstOrDefaultAsync(x => x.Id == offer.ShippingRequestBidId.Value);
                bid.IsAccepted = true;
                bid.price = bid.BasePrice = offer.CarrierPrice.Value;
                bid.ShippingRequestFk.BidStatus = ShippingRequestBidStatus.Closed;
            }
            else if (offer.ShippingRequestCarrierDirectPricingId != null)
            {
                var directPriceItem = await _shippingRequestsCarrierDirectPricingRepository.FirstOrDefaultAsync(offer.ShippingRequestCarrierDirectPricingId.Value);
                directPriceItem.Status = ShippingRequestsCarrierDirectPricingStatus.Accepted;
            }
        }


        #region Heleper
        private async Task Create(CreateOrEditTachyonPriceOfferDto input, ShippingRequest shippingRequest)
        {
            if (await IsExistingOffer(input.ShippingRequestId))
            {
                throw new UserFriendlyException(L("Cannot Create new offer, there is already existing offer message"));
            }
            if (input.TotalAmount < 1) throw new UserFriendlyException(L("ThePriceSholudBeNotLessThanOne"));

            if (input.DriectRequestForCarrierId.HasValue || input.ShippingRequestBidId.HasValue)
            {
                if (input.ActualCommissionValue < 1 && input.ActualPercentCommission < 1) throw new UserFriendlyException(L("TheCommissionValuevalueAndPercentCommissionMustNotBeLessThanOneAtTheSameTime"));
                else if (input.ActualCommissionValue < 0) throw new UserFriendlyException(L("TheCommissionValueSholudBeNotLessThanZero"));
                else if (input.ActualPercentCommission < 0) throw new UserFriendlyException(L("ThePercentCommissionSholudBeNotLessThanZero"));
            }

            await OfferMappingFromDirectRequestOrBidding(input, shippingRequest);
            TachyonPriceOffer tachyonPriceOffer = ObjectMapper.Map<TachyonPriceOffer>(input);

            await _commissionManager.CalculateAmountByOffer(tachyonPriceOffer, shippingRequest);

            await _tachyonPriceOfferRepository.InsertAsync(tachyonPriceOffer);
            await CurrentUnitOfWork.SaveChangesAsync();

            shippingRequest = await _shippingRequestRepository.FirstOrDefaultAsync(tachyonPriceOffer.ShippingRequestId);

            //send notification to shipper when create offer
            await _appNotifier.TachyonDealerOfferCreated(tachyonPriceOffer, shippingRequest);

        }

        private async Task Edit(CreateOrEditTachyonPriceOfferDto input, TachyonPriceOffer offer, ShippingRequest shippingRequest)
        {
            await OfferMappingFromDirectRequestOrBidding(input, shippingRequest);
            offer.CarrierPrice = input.CarrierPrice;
            offer.CarrirerTenantId = input.CarrirerTenantId;
            offer.ShippingRequestBidId = input.ShippingRequestBidId;
            offer.OfferStatus = OfferStatus.Accepted;
            SetSettings(offer, shippingRequest);


            offer.ShippingRequestFk.CarrierTenantId = offer.CarrirerTenantId;

            offer.ShippingRequestFk.VatAmount = offer.VatAmount.Value;
            offer.ShippingRequestFk.Price = offer.TotalAmount;
            offer.ShippingRequestFk.SubTotalAmount = offer.SubTotalAmount.Value;
            offer.ShippingRequestFk.CarrierPrice = offer.CarrierPrice.Value;
            offer.ShippingRequestFk.VatSetting = offer.VatSetting.Value;
            //ObjectMapper.Map(offer, offer.ShippingRequestFk);
            await _shippingRequestManager.SetToPostPrice(offer.ShippingRequestFk);
        }

        private async Task OfferMappingFromDirectRequestOrBidding(CreateOrEditTachyonPriceOfferDto input, ShippingRequest request)
        {
            if (input.DriectRequestForCarrierId.HasValue)
            {
                var direct = await _shippingRequestsCarrierDirectPricingRepository.FirstOrDefaultAsync(input.DriectRequestForCarrierId.Value);
                if (direct == null) throw new UserFriendlyException(L("TheCarrierDirectPricingIsNotFound"));
                input.CarrierPrice = direct.Price;
                input.CarrirerTenantId = direct.CarrirerTenantId;
                direct.Status = ShippingRequestsCarrierDirectPricingStatus.Accepted;
                if (request.IsBid)
                {
                    request.BidStatus = ShippingRequestBidStatus.Closed;
                }
            }
            else if (input.ShippingRequestBidId.HasValue)
            {
                var bid = await _shippingRequestBidsRepository
                    .GetAll()
                    .Where(x => x.Id == input.ShippingRequestBidId.Value && x.ShippingRequestId == request.Id)
                    .FirstOrDefaultAsync();
                if (bid == null) throw new UserFriendlyException(L("ThebidIsNotFound"));
                input.CarrierPrice = bid.BasePrice;
                input.CarrirerTenantId = bid.TenantId;
                await _shippingRequestBidManager.AcceptBidAndGoToPostPriceAsync(bid);
            }
        }
        private async Task<bool> IsExistingOffer(long shippingRequestId)
        {
            return await _tachyonPriceOfferRepository.GetAll()
                .AnyAsync(e => e.ShippingRequestId == shippingRequestId && e.OfferStatus != OfferStatus.Rejected);
        }
        private async Task<TachyonPriceOffer> GetTachyonPriceOffer(int id)
        {
            return await _tachyonPriceOfferRepository.FirstOrDefaultAsync(id);
        }
        private async Task<ShippingRequest> GetShippingRequestOnPrePriceStage(long requestId)
        {
            ShippingRequest shippingRequest = await _shippingRequestRepository.GetAll().Include(t => t.Tenant).FirstOrDefaultAsync(e => e.Id == requestId && e.IsTachyonDeal && (e.Status == ShippingRequestStatus.PrePrice || e.Status == ShippingRequestStatus.NeedsAction));
            if (shippingRequest == null) throw new UserFriendlyException(L("NoShippingRequest"));

            return shippingRequest;
        }

        private void SetSettings(TachyonPriceOffer tachyonPriceOffer, ShippingRequest shippingRequest)
        {
            ShippingRequestAmount shippingRequestAmount = new ShippingRequestAmount();
            _commissionManager.CalculateAmountByTachyonDealerRequestSettings(shippingRequestAmount, shippingRequest);
            tachyonPriceOffer.CommissionValueSetting = shippingRequestAmount.CommissionValueSetting;
            tachyonPriceOffer.PercentCommissionSetting = shippingRequestAmount.PercentCommissionSetting;
            tachyonPriceOffer.MinCommissionValueSetting = shippingRequestAmount.MinCommissionValueSetting;
            if (!tachyonPriceOffer.CarrirerTenantId.HasValue)
            {
                tachyonPriceOffer.PriceType = PriceType.GuesingPrice;
            }
            else if (tachyonPriceOffer.ShippingRequestBidId.HasValue)
            {
                tachyonPriceOffer.PriceType = PriceType.Bidding;
            }
            else
            {
                tachyonPriceOffer.PriceType = PriceType.DirectRequest;
            }
        }
        #endregion


    }
}
