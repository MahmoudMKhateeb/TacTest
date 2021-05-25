using Abp;
using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Features;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Invoices.Balances;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;
using TACHYON.Shipping.ShippingRequestBids.Dtos;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.ShippingRequestVases.Dtos;
using TACHYON.TachyonPriceOffers;
using TACHYON.Trucks;

namespace TACHYON.Shipping.ShippingRequestBids
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids)]
    public class ShippingRequestBidsAppService : TACHYONAppServiceBase, IShippingRequestBidsAppService
    {
        public ShippingRequestBidsAppService(IRepository<ShippingRequestBid, long> shippingRequestBidsRepository,
            IRepository<ShippingRequest, long> shippingRequestsRepository,
            IRepository<Tenant> tenantsRepository, IRepository<Truck, long> trucksRepository,
            BackgroundJobManager backgroundJobManager, CommissionManager commissionManager,
            IAppNotifier appNotifier, OfferManager offerManager, BalanceManager balanceManager, UserManager userManager, IRepository<Edition> editionRepository,
            ShippingRequestManager shippingRequestManager, ShippingRequestBidManager shippingRequestBidManager,
            TachyonPriceOffersAppService carrierPriceOfferService)
        {
            _shippingRequestBidsRepository = shippingRequestBidsRepository;
            _shippingRequestsRepository = shippingRequestsRepository;
            _tenantsRepository = tenantsRepository;
            _trucksRepository = trucksRepository;
            _appNotifier = appNotifier;
            _backgroundJobManager = backgroundJobManager;
            _commissionManager = commissionManager;
            _offerManager = offerManager;
            _balanceManager = balanceManager;
            _shippingRequestManager = shippingRequestManager;
            _userManager = userManager;
            _editionRepository = editionRepository;
            _shippingRequestBidManager = shippingRequestBidManager;
            _carrierPriceOfferService = carrierPriceOfferService;
        }

        private readonly IRepository<ShippingRequestBid, long> _shippingRequestBidsRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestsRepository;
        private readonly IRepository<Tenant> _tenantsRepository;
        private readonly IRepository<Truck, long> _trucksRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly BackgroundJobManager _backgroundJobManager;
        private readonly CommissionManager _commissionManager;
        private readonly OfferManager _offerManager;
        private readonly BalanceManager _balanceManager;
        private readonly ShippingRequestManager _shippingRequestManager;
        private readonly UserManager _userManager;
        private readonly IRepository<Edition> _editionRepository;
        private readonly ShippingRequestBidManager _shippingRequestBidManager;
        private readonly TachyonPriceOffersAppService _carrierPriceOfferService;


        /// <summary>
        ///     This is for shipper to view Shipping Request bids in view-shipping-request page.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [RequiresFeature(AppFeatures.Shipper, AppFeatures.TachyonDealer)]
        public virtual async Task<PagedResultDto<ShippingRequestBidDto>> GetAllShippingRequestBids(GetAllShippingRequestBidsInput input)
        {

            DisableTenancyFilters();
            IQueryable<ShippingRequestBid> filterShippingRequestsBids = _shippingRequestBidsRepository.GetAll()
                .Include(x => x.Tenant)
                .Where(e => e.ShippingRequestId == input.ShippingRequestId)
                .WhereIf(await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId && !x.ShippingRequestFk.IsTachyonDeal)
                .WhereIf(await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestFk.IsTachyonDeal)
                .WhereIf(input.MinPrice != null, e => e.price >= input.MinPrice)
                .WhereIf(input.MaxPrice != null, e => e.price <= input.MaxPrice);


            IQueryable<ShippingRequestBid> pagedAndFilteredShippingRequestsBids = filterShippingRequestsBids
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            IQueryable<ShippingRequestBidDto> shippingRequestBids = pagedAndFilteredShippingRequestsBids
                //todo Islam Create auto mapping here 
                .Select(o => new ShippingRequestBidDto
                {
                    Id = o.Id,
                    ShippingRequestId = o.ShippingRequestId,
                    IsAccepted = o.IsAccepted,
                    IsRejected = o.IsRejected,
                    price = o.price,
                    CreationTime = o.CreationTime,
                    CarrierName = o.Tenant.Name
                });

            int totalCount = await filterShippingRequestsBids.CountAsync();

            return new PagedResultDto<ShippingRequestBidDto>(totalCount, await shippingRequestBids.ToListAsync());
        }

        /// <summary>
        /// Shipper can cancel shipping request
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CancelBidShippingRequest(CancelBidShippingRequestInput input)
        {
            ShippingRequest bid = await _shippingRequestsRepository.GetAsync(input.ShippingRequestId);
            if (bid.BidStatus != ShippingRequestBidStatus.OnGoing)
            {
                ThrowShippingRequestIsNotOngoingError();
            }
            else
            {
                bid.BidStatus = ShippingRequestBidStatus.Cancled;
                bid.CloseBidDate = Clock.Now;
            }
        }


        public async Task<long> CreateOrEditShippingRequestBid(CreatOrEditShippingRequestBidDto input)
        {
            //DisableTenancyFilters();
            ShippingRequest item;
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                 item = await _shippingRequestsRepository.GetAll()
                    .Include(x=>x.ShippingRequestBids)
               .Where(x => x.BidStatus == ShippingRequestBidStatus.OnGoing)
               .FirstOrDefaultAsync(x => x.Id == input.ShippingRequestId);
            }
           

            if (item==null)
            {
                ThrowShippingRequestIsNotOngoingError();
            }
            

            if (input.Id == null)
            {
                if (!item.IsTachyonDeal && item.ShippingRequestBids.Count()==0)
                {
                    item.Status = ShippingRequestStatus.NeedsAction;
                }
                return await Create(input,item);
            }

            return await Edit(input,item);
        }

        /// <summary>
        /// #539 shipper accept carrier bid request 
        /// </summary>
        /// <param name="shippingRequestBidId"></param>
        /// <returns></returns>
        /// 
        [RequiresFeature(AppFeatures.Shipper)]
        public async Task AcceptShippingRequestBid(long shippingRequestBidId)
        {
            DisableTenancyFilters();

                ShippingRequestBid bid = await _shippingRequestBidsRepository.GetAll()
                    .Include(x => x.ShippingRequestFk)
                    .FirstOrDefaultAsync
                    (
                        x => x.Id == shippingRequestBidId &&
                        x.ShippingRequestFk.TenantId == AbpSession.TenantId &&
                        x.ShippingRequestFk.IsBid &&
                        !x.ShippingRequestFk.IsTachyonDeal &&
                        x.ShippingRequestFk.Status == ShippingRequestStatus.PrePrice &&
                        x.ShippingRequestFk.BidStatus == ShippingRequestBidStatus.OnGoing
                    );
                if (bid == null)
                {
                    ThrowShippingRequestIsNotOngoingError();
                }

                 await _shippingRequestBidManager.AcceptBidAndGoToPostPriceAsync(bid);


            //redirect automaic from front if the feature is tacyon dealer to CreateOrEditTachyonPriceOffer in tachyonpriceofferAppService

            //bid.IsAccepted = true;
            //bid.ShippingRequestFk.BidStatus = ShippingRequestBidStatus.Closed;
            //await _balanceManager.ShipperCanAcceptPrice(bid.ShippingRequestFk.TenantId,bid.price,bid.ShippingRequestId);
            //AssignShippingRequestInfo(bid.ShippingRequestFk, bid);
            //await _shippingRequestManager.SetToPostPrice(bid.ShippingRequestFk);


            //    //Reject the other bids of this shipping request by background job
            //    await _backgroundJobManager.EnqueueAsync<RejectOtherBidsJob, RejectOtherBidsJobArgs>
            //        (new RejectOtherBidsJobArgs { AcceptedBidId = bid.Id, ShippingReuquestId = bid.ShippingRequestId });


            //update shippingRequest final price
            //ShippingRequest shippingRequestItem =await GetShippingRequest(bid.ShippingRequestId);
            ////todo review this to commission task team
            //if (await IsEnabledAsync(AppFeatures.Shipper))
            //{
            //    AssignShippingRequestInfoAndGoToPostPrice(shippingRequestItem, bid);
            //    //#540 notification to carrier told bid accepted
            //    await _appNotifier.AcceptShippingRequestBid(new UserIdentifier(bid.TenantId, bid.CreatorUserId.Value), bid.ShippingRequestId);
            //}

            //else if (await IsEnabledAsync(AppFeatures.TachyonDealer))
            //{
            //    shippingRequestItem.CarrierTenantId = bid.TenantId;
            //    shippingRequestItem.CarrierPrice = bid.price;
            //    shippingRequestItem.CarrierPriceType = CarrierPriceType.TachyonDealerBidding;
            //    //check if there is expected offer tachyon deal sent to shipper
            //    var offer = await _offerManager.GetAcceptedAndWaitingForCarrierOffer(shippingRequestItem.Id);
            //    if (offer != null)
            //    {
            //        //go to post price
            //        _commissionManager.AssignShippingRequestActualCommissionAndGoToPostPrice(shippingRequestItem,
            //            offer);

            //        //to do send notification to shipper
            //    }
            //}


        }

        //private void AssignShippingRequestInfo(ShippingRequest shippingRequestItem,ShippingRequestBid bid)
        //{
        //    shippingRequestItem.CarrierTenantId = bid.TenantId;
        //    shippingRequestItem.Price = bid.price; //bid price that biddingCommission added to the base
        //  //  shippingRequestItem.Status = ShippingRequestStatus.PostPrice;
        //    shippingRequestItem.CarrierPriceType = CarrierPriceType.ShipperBidding;
        //    shippingRequestItem.ActualCommissionValue = bid.ActualCommissionValue;
        //    shippingRequestItem.ActualPercentCommission = bid.ActualPercentCommission;
        //    shippingRequestItem.ActualMinCommissionValue = bid.ActualMinCommissionValue;
        //    shippingRequestItem.TotalCommission = bid.TotalCommission;
        //    shippingRequestItem.VatAmount = bid.VatAmount;
        //    shippingRequestItem.SubTotalAmount = bid.PriceSubTotal;
        //}
        private async Task<ShippingRequest> GetShippingRequest(long id)
        {
            return await _shippingRequestsRepository.FirstOrDefaultAsync(id);
        }

       
        //#541
        //todo add paging and sorting here 
        [RequiresFeature(AppFeatures.Carrier)]
        public async Task<List<GetAllCarrierShippingRequestBidsOutput>> GetAllCarrierShippingRequestBids()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                IQueryable<ShippingRequestBid> shippingRequestBids = _shippingRequestBidsRepository.GetAll()
                    .Include(x => x.ShippingRequestFk)
                    .Include(x => x.ShippingRequestFk.Tenant)
                    .Where(x => x.TenantId == AbpSession.TenantId);


                IQueryable<GetAllCarrierShippingRequestBidsOutput> list = shippingRequestBids
                    .Select(o => new GetAllCarrierShippingRequestBidsOutput
                    {
                        ShipperTenancyName = o.ShippingRequestFk.Tenant.Name,
                        ShippingRequestBidDto = ObjectMapper.Map<ShippingRequestBidDto>(o),
                        ShippingRequestDto = ObjectMapper.Map<ShippingRequestDto>(o.ShippingRequestFk)
                    });

                return await list.ToListAsync();
            }
        }

        [RequiresFeature(AppFeatures.Carrier)]
        public async Task CancelShippingRequestBid(CancelShippingRequestBidInput input)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                ShippingRequestBid bid = await _shippingRequestBidsRepository.GetAll()
                    .Where(x=>x.TenantId==AbpSession.TenantId)
                    .Include(x => x.ShippingRequestFk)
                    .FirstOrDefaultAsync(x => x.Id == input.ShippingRequestBidId);

                //check if the bid is already canceled
                if (bid.IsCancled)
                {
                    throw new UserFriendlyException(L("bid is already canceled message"));
                }

                //Check if Shipping Request is not ongoing -- add cancel reason
                if (bid.ShippingRequestFk.BidStatus != ShippingRequestBidStatus.OnGoing)
                {
                    if (bid.CancledReason.IsNullOrWhiteSpace())
                    {
                        throw new UserFriendlyException(L("CancelReason is required"));
                    }

                    bid.CancledReason = input.CancledReason;
                }

                //Cancel Bid
                bid.IsCancled = true;
                bid.CanceledDate = Clock.Now;


                //notification to shipper when Carrier cancel his bid in his Shipping Request
                await _appNotifier.CancelBidRequest(
                    new UserIdentifier(bid.ShippingRequestFk.TenantId, bid.ShippingRequestFk.CreatorUserId.Value),
                    bid.ShippingRequestId,
                    bid.Id);

            }
        }

        [RequiresFeature(AppFeatures.Carrier)]
        public  async Task<PagedResultDto<GetAllBidShippingRequestsForCarrierOutput>> GetAllBidShippingRequestsForCarrier(GetAllBidsShippingRequestForCarrierInput input)
        {
            DisableTenancyFilters();
      
                IQueryable<ShippingRequest> filterBidShippingRequests = _shippingRequestsRepository.GetAll()
                    .Include(x => x.TrucksTypeFk)
                    .Include(x => x.GoodCategoryFk)
                    .ThenInclude(x=>x.Translations)
                    /*.Include(x => x.ShippingRequestBids.Where(x=>x.TenantId==AbpSession.TenantId))*/
                    /*.Include(x => x.ShippingRequestVases)
                    .ThenInclude(x => x.VasFk)*/
                    .Include(x => x.OriginCityFk)
                    .ThenInclude(x=>x.CountyFk)
                    .Include(x => x.DestinationCityFk)
                    .ThenInclude(x=>x.CountyFk)
                    .Include(x => x.Tenant)
                    .Where(x => x.IsBid)
                    .Where(x=>x.BidStatus== ShippingRequestBidStatus.OnGoing)
                    .WhereIf(input.TruckTypeId.HasValue, x => x.TrucksTypeId == input.TruckTypeId)
                    .WhereIf(input.TransportType.HasValue, x => x.TransportTypeId != null && x.TransportTypeId == input.TransportType)
                    .WhereIf(input.IsMyAssignedBidsOnly.HasValue && input.IsMyAssignedBidsOnly==true, x=>x.ShippingRequestBids.Any(y=>y.IsAccepted== true))
                    //Filter
                    .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), x =>  x.CarrierTenantId==AbpSession.TenantId)

                    //Get Shipping Requests that carrier bid to them only
                    .WhereIf(input.IsMyBidsOnly, x => x.ShippingRequestBids.Any(b => b.TenantId == AbpSession.TenantId));


                if (input.IsMatchingOnly)
                {
                    //todo Islam : improve to filter by full truck type
                    //Get all Carrier trucktype list to filter with
                    List<long?> tenantTrucks = await _trucksRepository.GetAll()
                        .Where(x => x.TenantId == AbpSession.TenantId)
                        .Select(y => y.TrucksTypeId)
                        .ToListAsync();

                    // select shipping requests that only matches Carrier truck type
                    filterBidShippingRequests = filterBidShippingRequests
                        .Where(x => tenantTrucks.Contains(x.TrucksTypeId));
                }


                List<ShippingRequest> pagedAndFilteredBidShippingRequests = await filterBidShippingRequests
                    .OrderBy(input.Sorting ?? "id asc")
                    .PageBy(input).ToListAsync();

          
                //IEnumerable<GetAllBidShippingRequestsForCarrierOutput> shippingRequestBids = (await pagedAndFilteredBidShippingRequests.ToListAsync())
                //    .Select( o  => new GetAllBidShippingRequestsForCarrierOutput
                //    {
                //        ShippingRequestId = o.Id,
                //        BidStartDate = o.BidStartDate,
                //        ShippingRequestBidStatusName =Enum.GetName(typeof(ShippingRequestBidStatus),o.BidStatus),
                //        ShipperName = o.Tenant.Name,
                //        TruckTypeDisplayName = o.TrucksTypeFk.DisplayName,
                //        GoodCategoryName = o.GoodCategoryFk.DisplayName,
                //        MyBidPrice = GetCarrirerPrice(o.Id).GetAwaiter().GetResult(),
                //        MyBidId = o.ShippingRequestBids.FirstOrDefault()?.Id,
                //        SourceCityName = o.OriginCityFk?.DisplayName,
                //        DestinationCityName = o.DestinationCityFk?.DisplayName,
                //        ShippingRequestVasesDto=o.ShippingRequestVases.Select(e=>new GetShippingRequestVasForViewDto
                //        {
                //            ShippingRequestVas =ObjectMapper.Map<ShippingRequestVasDto>(e),
                //            VasName = e.VasFk.Name
                //        }),
                //        TotalWeight = o.TotalWeight,
                //        NumberOfTrips = o.NumberOfTrips
                //    });

            List<GetAllBidShippingRequestsForCarrierOutput> shippingRequestBids=new List<GetAllBidShippingRequestsForCarrierOutput>();
            pagedAndFilteredBidShippingRequests.ForEach(   o =>
            {
                var bid =  GetCarrirerBid(o.Id);
                shippingRequestBids.Add(new GetAllBidShippingRequestsForCarrierOutput()
                {
                    ShippingRequestId = o.Id,
                    BidStartDate = o.BidStartDate,
                    ShippingRequestBidStatusName = Enum.GetName(typeof(ShippingRequestBidStatus), o.BidStatus),
                    ShipperName = o.Tenant.Name,
                    TruckTypeDisplayName = o.TrucksTypeFk.DisplayName,
                    GoodCategoryName = ObjectMapper.Map<GoodCategoryDto>(o.GoodCategoryFk).DisplayName,// o.GoodCategoryFk.DisplayName,
                    MyBidPrice = bid?.BasePrice,
                    MyBidId = bid?.Id,
                    SourceCityName = o.OriginCityFk?.DisplayName,
                    DestinationCityName = o.DestinationCityFk?.DisplayName,
                    TotalWeight = o.TotalWeight,
                    NumberOfTrips = o.NumberOfTrips
                });
            });



                int totalCount = await filterBidShippingRequests.CountAsync();

                return new PagedResultDto<GetAllBidShippingRequestsForCarrierOutput>(totalCount, shippingRequestBids.ToList());
            
        }
        private ShippingRequestBid GetCarrirerBid(long ShippingRequestId)
        {
            return  _shippingRequestBidsRepository.FirstOrDefault(x => x.TenantId == AbpSession.TenantId && x.ShippingRequestId == ShippingRequestId);
        }
   
        //#538
        [RequiresFeature(AppFeatures.Carrier)]
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids_Create)]
        private async Task<long> Create(CreatOrEditShippingRequestBidDto input, ShippingRequest shippingRequest)
        {
            var count = await _shippingRequestBidsRepository.CountAsync(x => x.ShippingRequestId == input.ShippingRequestId && x.TenantId==AbpSession.TenantId);

            if (count > 0)
            {
                throw new UserFriendlyException(L("You have already Bid to this shipping before message"));
            }

            var basePrice = input.BasePrice;
            ShippingRequestBid shippingRequestBid = ObjectMapper.Map<ShippingRequestBid>(input);
            if (AbpSession.TenantId != null)
            {
                shippingRequestBid.TenantId = (int)AbpSession.TenantId;
            }

            
            await _commissionManager.AddCommissionInfoAfterCarrierBid(shippingRequestBid);
            await _shippingRequestBidsRepository.InsertAndGetIdAsync(shippingRequestBid);
            shippingRequest.TotalBids += 1;
            shippingRequest.Status = ShippingRequestStatus.NeedsAction;
            await CurrentUnitOfWork.SaveChangesAsync();

            await SendNotificationAfterBid(shippingRequest, shippingRequestBid,true);

            return shippingRequestBid.Id;
        }


        [RequiresFeature(AppFeatures.Carrier)]
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids_Edit)]
        private async Task<long> Edit(CreatOrEditShippingRequestBidDto input,ShippingRequest shippingRequest)
        {
            ShippingRequestBid item = await _shippingRequestBidsRepository.FirstOrDefaultAsync(x=>x.Id== input.Id.Value && x.TenantId==AbpSession.TenantId);
            ObjectMapper.Map(input, item);
            await _commissionManager.AddCommissionInfoAfterCarrierBid(item);

            await SendNotificationAfterBid(shippingRequest, item,false);

            return item.Id;


        }

        private async Task SendNotificationAfterBid(ShippingRequest shippingRequest, ShippingRequestBid shippingRequestBid,bool IsCreateOperation)
        {
            if (!shippingRequest.IsTachyonDeal)
            {
                //notification to shipper when Carrier create new bid in his Shipping Request
                if (IsCreateOperation)
                {
                    await _appNotifier.CreateBidRequest(
                        new UserIdentifier(shippingRequest.TenantId, shippingRequest.CreatorUserId.Value),
                        shippingRequestBid.Id);
                }
                else
                {
                    await _appNotifier.UpdateBidRequest(
                        new UserIdentifier(shippingRequest.TenantId, shippingRequest.CreatorUserId.Value),
                        shippingRequestBid.Id);
                }
            }
            else
            {
                //send notification to tachyon dealer
                var edition = await _editionRepository.FirstOrDefaultAsync(x => x.DisplayName.ToLower() == TACHYONConsts.TachyonDealerEdtionName.ToLower());
                var tenant = await _tenantsRepository.FirstOrDefaultAsync(x => x.EditionId == edition.Id);
                var tachyonDealerUser = await _userManager.GetAdminByTenantIdAsync(tenant.Id);

                if (IsCreateOperation)
                {
                    await _appNotifier.CreateBidRequest(
                        new UserIdentifier(tenant.Id, tachyonDealerUser.Id),
                        shippingRequestBid.Id);
                }
                else
                {
                    await _appNotifier.UpdateBidRequest(
                        new UserIdentifier(tenant.Id, tachyonDealerUser.Id),
                        shippingRequestBid.Id);
                }
            }
        }

        private void ThrowShippingRequestIsNotOngoingError()
        {
            throw new UserFriendlyException(L("The Bid must be Ongoing message"));
        }



    }
}