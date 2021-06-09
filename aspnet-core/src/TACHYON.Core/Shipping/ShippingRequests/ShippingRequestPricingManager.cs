//using Abp;
//using Abp.Application.Features;
//using Abp.Application.Services.Dto;
//using Abp.Collections.Extensions;
//using Abp.Configuration;
//using Abp.Domain.Repositories;
//using Abp.Domain.Uow;
//using Abp.Linq.Extensions;
//using Abp.Runtime.Session;
//using Abp.Threading;
//using Abp.UI;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using TACHYON.Authorization.Users;
//using TACHYON.Features;
//using TACHYON.Invoices.Balances;
//using TACHYON.Notifications;
//using TACHYON.Shipping.DirectRequests;
//using TACHYON.Shipping.ShippingRequests.ShippingRequestsPricing.Dto;

//namespace TACHYON.Shipping.ShippingRequests
//{
//    public class ShippingRequestPricingManager : TACHYONDomainServiceBase
//    {
//        private IRepository<ShippingRequestPricing, long> _shippingRequestPricingRepository;
//        private IRepository<ShippingRequest, long> _shippingRequestsRepository;
//        private readonly IRepository<ShippingRequestDirectRequest, long> _shippingRequestDirectRequestRepository;
//        private readonly IAppNotifier _appNotifier;
//        private readonly ISettingManager _settingManager;
//        private readonly IFeatureChecker _featureChecker;
//        private readonly IAbpSession _abpSession;
//        private readonly BalanceManager _balanceManager;
//        private readonly ShippingRequestManager _shippingRequestManager;
//        private ShippingRequestDirectRequest _directRequest;

//        public ShippingRequestPricingManager(IRepository<ShippingRequestPricing, long> shippingRequestPricingRepository, IAppNotifier appNotifier, ISettingManager settingManager, IFeatureChecker featureChecker, IRepository<ShippingRequest, long> shippingRequestsRepository, IAbpSession abpSession, BalanceManager balanceManager, ShippingRequestManager shippingRequestManager, IRepository<ShippingRequestDirectRequest, long> shippingRequestDirectRequestRepository)
//        {
//            _shippingRequestPricingRepository = shippingRequestPricingRepository;
//            _appNotifier = appNotifier;
//            _settingManager = settingManager;
//            _featureChecker = featureChecker;
//            _shippingRequestsRepository = shippingRequestsRepository;
//            _abpSession = abpSession;
//            _balanceManager = balanceManager;
//            _shippingRequestManager = shippingRequestManager;
//            _shippingRequestDirectRequestRepository = shippingRequestDirectRequestRepository;
//        }

//        public async Task CreateOrEdit(CreateOrEditPricingInput Input)
//        {
//            DisableTenancyFilters();
            
//            var shippingRequest = await _shippingRequestsRepository
//                                        .GetAll()
//                                        .Include(x=>x.ShippingRequestVases)
//                                        .FirstOrDefaultAsync(r => r.Id == Input.ShippingRequestId && (r.Status == ShippingRequestStatus.NeedsAction || r.Status == ShippingRequestStatus.PrePrice));
//            if (shippingRequest == null) throw new UserFriendlyException(L("TheShippingRequestNotFound"));
   
//            if (Input.Channel== ShippingRequestPricingChannel.MarketPlace)
//            {
//                MarketPlaceCanAccess(Input, shippingRequest);
//            }
//            else
//            {
//                await DirectRequestCanAccess(Input, shippingRequest);
//            }
//            var offer = GetCarrierPricingOrNull(Input.ShippingRequestId);
//            if (offer==null)
//            {
//                await Create(Input, shippingRequest);
//            }
//            else
//            {
//                await Update(Input, shippingRequest, offer);
//            }
//        }

//        public async Task Delete(EntityDto Input)
//        {
//            DisableTenancyFilters();
//            var pricing = await _shippingRequestPricingRepository
//                .GetAll()
//                .Include(x=> x.ShippingRequestFK)
//                .FirstOrDefaultAsync(x => x.Id == Input.Id && x.TenantId == _abpSession.TenantId.Value && x.ShippingRequestFK.Status== ShippingRequestStatus.NeedsAction);
//            if (pricing == null) throw new UserFriendlyException(L("TheRecordNotFound"));
//            var request = pricing.ShippingRequestFK;
//            if (pricing.Channel == ShippingRequestPricingChannel.MarketPlace)
//            {
//                if (pricing.ShippingRequestFK.BidStatus != ShippingRequestBidStatus.OnGoing) throw new UserFriendlyException(L("TheRecordNotFound"));
//                request.TotalBids -= 1;
//            }
//            else if (pricing.Channel == ShippingRequestPricingChannel.DirectRequest) 
//            {
//                var directRequest = await _shippingRequestDirectRequestRepository.FirstOrDefaultAsync(x => x.Id == pricing.SourceId);
//                if (directRequest != null) directRequest.Status = ShippingRequestDirectRequestStatus.New;
//            }

//            if (! await _shippingRequestPricingRepository.GetAll().AnyAsync(x=>x.ShippingRequestId== request.Id && x.Id != Input.Id))
//            {
//                request.Status = ShippingRequestStatus.PrePrice;
//            }
//            await _shippingRequestPricingRepository.DeleteAsync(pricing);

//        }


//        //Shipper Accept Offer
//        public async Task AcceptOffer(long Id)
//        {
//            DisableTenancyFilters();
//            var offer = await _shippingRequestPricingRepository
//                .GetAll()
//                .Include(x => x.ShippingRequestFK)
//                .Include(t=>t.Tenant)
//                  .ThenInclude(e=>e.Edition)
//                .FirstOrDefaultAsync(x=> x.Id== Id &&
//                x.ShippingRequestFK.TenantId == _abpSession.TenantId.Value &&
//                x.Status== ShippingRequestPricingStatus.New &&
//                x.ShippingRequestFK.Status == ShippingRequestStatus.NeedsAction &&
//                (!x.ShippingRequestFK.IsTachyonDeal || (x.ShippingRequestFK.IsTachyonDeal && x.ParentId.HasValue)));
//            if (offer == null)  throw new UserFriendlyException(L("TheOfferIsNotFound"));
           

//            var request = offer.ShippingRequestFK;
//            var edtion = offer.Tenant.Edition;
//            ShippingRequestPricing parentOffer = default;
//            //Check if shipper have enough balance to pay 
//            await _balanceManager.ShipperCanAcceptOffer(offer);

//            List<UserIdentifier> Users = new List<UserIdentifier>();
//            /// Check if the offer send by TAD on market place
//            if (edtion.Name.ToLower()== AppConsts.TachyonEditionName && offer.Channel== ShippingRequestPricingChannel.MarketPlace)
//            {
//                offer.Status = ShippingRequestPricingStatus.AcceptedAndWaitingForCarrier;
//                request.Status = ShippingRequestStatus.AcceptedAndWaitingCarrier;
//                request.IsBid = false;
//                request.BidStatus = ShippingRequestBidStatus.StandBy;
//                request.BidStartDate = default;
//                request.BidEndDate = default;
//            }
//            /// Check if offer has carrier from parent offer comming
//            else if (offer.ParentId.HasValue || !offer.ShippingRequestFK.IsTachyonDeal)
//            {
//                offer.Status = ShippingRequestPricingStatus.Accepted;
//                request.Status = ShippingRequestStatus.Completed;
//                if (offer.ParentId.HasValue)
//                {
//                    parentOffer = await GetOfferById(offer.ParentId.Value);
//                }
//                request.CarrierTenantId = parentOffer != null? parentOffer.TenantId : offer.TenantId;
//                if (request.IsBid) request.BidStatus=ShippingRequestBidStatus.Closed;
//            }
//            else //TAD still need to find carrier ro assign to shipping request
//            {
//                offer.Status = ShippingRequestPricingStatus.AcceptedAndWaitingForCarrier;
//                request.Status = ShippingRequestStatus.AcceptedAndWaitingCarrier;
               
//            }

//            await SetShippingRequestPricing(offer);
//            await _appNotifier.ShipperAcceptedOffers(offer, parentOffer);
//        }
//        /// <summary>
//        /// Get parent offer when the current offer is tachyon deal
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>

//        public async Task<ShippingRequestPricing> GetOfferById(long id)
//        {
//            return (await _shippingRequestPricingRepository.FirstOrDefaultAsync(x => x.Id == id));
//        }

//        public async Task<ShippingRequestPricing> GetOfferBySource(long sourceId, ShippingRequestPricingChannel channel)
//        {
//            return (await _shippingRequestPricingRepository.FirstOrDefaultAsync(x => x.SourceId == sourceId && x.Channel== channel));
//        }
//        #region Helper



//        /// <summary>
//        /// Find carrier tenant id to assing to shipping request if the offer direct from carrier or by TAD
//        /// </summary>
//        /// <param name="offer"></param>
//        /// <returns></returns>
//        private async Task<int> GetCarrierTenantId(ShippingRequestPricing offer)
//        {
//            if (!offer.ShippingRequestFK.IsTachyonDeal) return offer.TenantId;
//            return (await _shippingRequestPricingRepository.FirstOrDefaultAsync(x => x.Id == offer.ParentId)).TenantId;
//        }
//        /// <summary>
//        /// Set the shipping reqquest prices
//        /// </summary>
//        /// <param name="offer"></param>
//        /// <returns></returns>
//        private Task SetShippingRequestPricing(ShippingRequestPricing offer)
//        {
//            var request = offer.ShippingRequestFK;
//            request.Price = offer.TotalAmountWithCommission;
//            request.SubTotalAmount = offer.SubTotalAmountWithCommission;
//            request.VatAmount = offer.VatAmountWithCommission;
//            request.VatSetting = offer.TaxVat;
//            request.TotalCommission = offer.TotalAmountWithCommission;

//            return Task.CompletedTask;
//        }
//        /// <summary>
//        /// If the current user login have feature carrier get the offer 
//        /// </summary>
//        /// <param name="requestId"></param>
//        /// <returns></returns>
//        public ShippingRequestPricing GetCarrierPricingOrNull(long requestId)
//        {
//            return _shippingRequestPricingRepository.GetAll().Include(x => x.ShippingRequestVasesPricing).FirstOrDefault(x => x.ShippingRequestId == requestId && x.TenantId == _abpSession.TenantId.Value);
//        }
//        /// <summary>
//        /// Check If the shipping request in market place to access to add offer
//        /// </summary>
//        /// <param name="Input"></param>
//        /// <param name="shippingRequest"></param>
//        private void MarketPlaceCanAccess(CreateOrEditPricingInput Input, ShippingRequest shippingRequest)
//        {
//            if (shippingRequest.BidStatus != ShippingRequestBidStatus.OnGoing) throw new UserFriendlyException(L("The Bid must be Ongoing"));
//            if (_featureChecker.IsEnabled(AppFeatures.TachyonDealer) && shippingRequest.IsTachyonDeal) throw new UserFriendlyException(L("YouCanNotMakeBidFromMarketPlaceWhenTheShippingManageByTAD"));

//        }

//        /// <summary>
//        /// Check the carrier of have direct request to access to add offer
//        /// </summary>
//        /// <param name="Input"></param>
//        /// <param name="shippingRequest"></param>
//        /// <returns></returns>
//        private async Task DirectRequestCanAccess(CreateOrEditPricingInput Input, ShippingRequest shippingRequest)
//        {
//            _directRequest = await _shippingRequestDirectRequestRepository.FirstOrDefaultAsync(x => x.CarrierTenantId == _abpSession.TenantId.Value && x.ShippingRequestId == Input.ShippingRequestId);
//            if (_directRequest==null) throw new UserFriendlyException(L("YouDoNotHaveDirectRequest"));
//            if (_directRequest.Status == ShippingRequestDirectRequestStatus.Accepted) throw new UserFriendlyException(L("YouCanNoCreateOrEditPriceWhenThePriceAccepted"));
//            Input.SourceId = _directRequest.Id;
//        }
//        private async Task Create(CreateOrEditPricingInput Input, ShippingRequest shippingRequest)
//        {

//            var Pricing = ObjectMapper.Map<ShippingRequestPricing>(Input);
//            Pricing.ShippingRequestVasesPricing = await GetListOfVases(Input, shippingRequest);
//            Pricing.ShippingRequestFK = shippingRequest;
//            Pricing.Calculate(_featureChecker, _settingManager, shippingRequest);

//            Pricing.Id = await _shippingRequestPricingRepository.InsertAndGetIdAsync(Pricing);
//            if (shippingRequest.Status != ShippingRequestStatus.NeedsAction) shippingRequest.Status = ShippingRequestStatus.NeedsAction;
//           if (Pricing.Channel== ShippingRequestPricingChannel.MarketPlace) shippingRequest.TotalBids += 1;
//           if (Pricing.Channel == ShippingRequestPricingChannel.DirectRequest) _directRequest.Status = ShippingRequestDirectRequestStatus.Response;
//            AsyncHelper.RunSync(() => _appNotifier.ShippingRequestSendOfferWhenAddPrice(Pricing, GetCurrentTenant(_abpSession).companyName));

//        }
//        private async Task Update(CreateOrEditPricingInput Input, ShippingRequest shippingRequest,ShippingRequestPricing Pricing)
//        {
//            if (Pricing.Status == ShippingRequestPricingStatus.Accepted) throw new UserFriendlyException(L("YourPriceAlreadyAcceptedYouCanEdit"));
//            ObjectMapper.Map(Input, Pricing);
//            Pricing.ShippingRequestVasesPricing.Clear();
//            Pricing.ShippingRequestVasesPricing = await GetListOfVases(Input, shippingRequest);
//            Pricing.Calculate(_featureChecker, _settingManager, shippingRequest);
//            if (Pricing.IsView)
//            {
//                Pricing.IsView = false;
//                Pricing.ShippingRequestFK = shippingRequest;
//                AsyncHelper.RunSync(()=>_appNotifier.ShippingRequestSendOfferWhenUpdatePrice(Pricing, GetCurrentTenant(_abpSession).companyName));

//            }
//           await _shippingRequestPricingRepository.UpdateAsync(Pricing);
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="Input"></param>
//        /// <param name="shippingRequest"></param>
//        /// <returns></returns>
//        private Task<List<ShippingRequestVasPricing>> GetListOfVases(CreateOrEditPricingInput Input, ShippingRequest shippingRequest)
//        {
//            List<ShippingRequestVasPricing> ShippingRequestVasesPricing = new List<ShippingRequestVasPricing>();
//            if (Input.ShippingRequestVasPricing.Count != shippingRequest.ShippingRequestVases.Count)
//            {
//                throw new UserFriendlyException(L("YouSholudAddPricesForAllVases"));
//            }
//            if (Input.ShippingRequestVasPricing.Any(x => x.Price <= 0)) throw new UserFriendlyException(L("ThePriceMustBeGreaterThanZero"));
//            foreach (var vas in shippingRequest.ShippingRequestVases)
//            {
//                var vasdto = Input.ShippingRequestVasPricing.FirstOrDefault(x => x.VasId == vas.Id);
//                if (vasdto == null) throw new UserFriendlyException(L("YouSholudAddVasRelatedWithShippingRequest"));

//                ShippingRequestVasesPricing.Add(new ShippingRequestVasPricing()
//                {
//                    ShippingRequestVasFK = vas,
//                    VasPrice = vasdto.Price
//                }); ;
//            }
//            return Task.FromResult(ShippingRequestVasesPricing);
//        }

//        #endregion
//    }
//}
