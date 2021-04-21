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
using TACHYON.Shipping.ShippingRequests;
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
        private readonly IAppNotifier _appNotifier;
        public TachyonPriceOffersAppService(IRepository<TachyonPriceOffer> tachyonPriceOfferRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository, CommissionManager commissionManager, BalanceManager balanceManager,
            ShippingRequestManager shippingRequestManager,
            IAppNotifier appNotifier)
        {
            _tachyonPriceOfferRepository = tachyonPriceOfferRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _commissionManager = commissionManager;
            _balanceManager = balanceManager;
            _shippingRequestManager = shippingRequestManager;
            _appNotifier = appNotifier;
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
            var Offer = await _tachyonPriceOfferRepository.FirstOrDefaultAsync(x => x.ShippingRequestId == input.ShippingRequestId && x.OfferStatus == OfferStatus.AcceptedAndWaitingForCarrier);

            if (Offer == null)
            {
                await Create(input, Request);
            }
            else
            {
                if (!input.CarrirerTenantId.HasValue) throw new UserFriendlyException(L("ItShouldBeHaveCarrirer"));
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
            if (offer != null)
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
                 .ThenInclude(x=>x.Tenant)
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
                await _appNotifier.TachyonDealOfferRejectedByShipper(offer);
            }
            //var offer = await GetTachyonPriceOffer((input.Id));

            //if (input.IsAccepted)
            //{
            //    if (!await IsPendingOfferAsyn(offer.ShippingRequestId))
            //    {
            //        throw new UserFriendlyException(L("Offer should be in pending status"));
            //    }

            //    //check if assigned carrier exists ..
            //    if (await IsCarrierAssigned(offer.ShippingRequestId))
            //    {
            //        //post price
            //        var shippingRequestItem =
            //            await _shippingRequestRepository.FirstOrDefaultAsync(offer.ShippingRequestId);

            //      _commissionManager.AssignShippingRequestActualCommissionAndGoToPostPrice(shippingRequestItem,
            //     offer);
            //        await _balanceManager.ShipperCanAcceptPrice(shippingRequestItem);
            //        await _shippingRequestManager.SendSmsToReceivers(shippingRequestItem);

            //        //to do send notification to tachyonDealer and shipper
            //    }
            //    else
            //    {
            //        offer.OfferStatus = OfferStatus.AcceptedAndWaitingForCarrier;
            //        //to do send notification to tachyonDealer
            //    }
            //}
            //else
            //{
            //    offer.OfferStatus = OfferStatus.Rejected;
            //    offer.RejectedReason = input.RejectedReason;
            //}
        }


        #region Heleper
        private async Task Create(CreateOrEditTachyonPriceOfferDto input, ShippingRequest shippingRequest)
        {
            if (await IsExistingOffer(input.ShippingRequestId))
            {
                throw new UserFriendlyException(L("Cannot Create new offer, there is already existing offer message"));
            }
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
            //if (await IsPendingOfferAsyn(input.ShippingRequestId))
            //{
            //    var item =await _tachyonPriceOfferRepository.FirstOrDefaultAsync(input.Id.Value);
            //    ObjectMapper.Map(input, item);
            //}
            //  ObjectMapper.Map(input, offer);
            offer.CarrierPrice = input.CarrierPrice;
            offer.CarrirerTenantId = input.CarrirerTenantId;
            offer.ShippingRequestBidId = input.ShippingRequestBidId;
            offer.OfferStatus = OfferStatus.Accepted;
            SetSettings(offer, shippingRequest);
            ObjectMapper.Map(offer, offer.ShippingRequestFk);
            await _shippingRequestManager.SetToPostPrice(offer.ShippingRequestFk);



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
            ShippingRequest shippingRequest = await _shippingRequestRepository.FirstOrDefaultAsync(e => e.Id == requestId && e.IsTachyonDeal && e.Status == ShippingRequestStatus.PrePrice);
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

        #region Removed
        //[RequiresFeature(AppFeatures.TachyonDealer)]
        //public async Task<GetTachyonPriceOfferForEditOutput> GetTachyonPriceForEdit(EntityDto entity)
        //{
        //    var item = await GetTachyonPriceOffer(entity.Id);
        //    return new GetTachyonPriceOfferForEditOutput
        //    {
        //        createOrEditTachyonPriceOffer = ObjectMapper.Map<CreateOrEditTachyonPriceOfferDto>(item)
        //    };
        //}
        //private async Task<bool> IsPendingOfferAsyn(long shippingRequestId)
        //{
        //    var item = await _tachyonPriceOfferRepository.GetAll()
        //        .Where(e => e.ShippingRequestId == shippingRequestId)
        //        .Where(e => e.OfferStatus == OfferStatus.Pending)
        //        .FirstOrDefaultAsync();
        //    return item != null;
        //}

        //private async Task<bool> IsPendingOfferAsyn(int id)
        //{
        //    var item = await _tachyonPriceOfferRepository.GetAll()
        //        .Where(e => e.Id == id)
        //        .Where(e => e.OfferStatus == OfferStatus.Pending)
        //        .FirstOrDefaultAsync();
        //    return item != null;
        //}

        //private async Task<bool> IsAcceptedOfferAsync(long shippingRequestId)
        //{
        //    var item = await _tachyonPriceOfferRepository.GetAll()
        //        .Where(e => e.ShippingRequestId == shippingRequestId)
        //        .Where(e => e.OfferStatus == OfferStatus.Accepted || e.OfferStatus == OfferStatus.AcceptedAndWaitingForCarrier)
        //        .FirstOrDefaultAsync();
        //    return item != null;
        //}

        //private async Task<bool> IsTachyonDealShippingRequestAsync(long shippingRequestId)
        //{
        //    using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
        //    {
        //        var item=await _shippingRequestRepository.FirstOrDefaultAsync(e => e.Id == shippingRequestId);
        //        return item.IsTachyonDeal;
        //    }
        //}



        //private async Task<CarrierPriceType> GetCarrierPriceType(long shippingRequestId)
        //{
        //    DisableTenancyFilters();
        //    var shippingRequest = await _shippingRequestRepository.FirstOrDefaultAsync(e => e.Id == shippingRequestId);
        //    return shippingRequest.CarrierPriceType;
        //}

        //private async Task<bool> IsCarrierAssigned(long shippingRequestId)
        //{
        //    DisableTenancyFilters();
        //    var shippingRequest = await _shippingRequestRepository.FirstOrDefaultAsync(e => e.Id == shippingRequestId);
        //    return shippingRequest.CarrierPrice !=0;
        //}

        #endregion
    }
}
