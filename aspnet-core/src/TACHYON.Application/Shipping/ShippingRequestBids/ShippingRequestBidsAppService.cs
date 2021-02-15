using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp;
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
using Org.BouncyCastle.Crypto.Generators;
using TACHYON.AddressBook.Dtos;
using TACHYON.Authorization;
using TACHYON.Features;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;
using TACHYON.Shipping.ShippingRequestBids.Dtos;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Trucks;

namespace TACHYON.Shipping.ShippingRequestBids
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids)]
    public class ShippingRequestBidsAppService : TACHYONAppServiceBase, IShippingRequestBidsAppService
    {
        public ShippingRequestBidsAppService(IRepository<ShippingRequestBid, long> shippingRequestBidsRepository,
            IRepository<ShippingRequest, long> shippingRequestsRepository,
            IRepository<Tenant> tenantsRepository, IRepository<Truck, long> trucksRepository,
            BackgroundJobManager backgroundJobManager,
            IAppNotifier appNotifier)
        {
            _shippingRequestBidsRepository = shippingRequestBidsRepository;
            _shippingRequestsRepository = shippingRequestsRepository;
            _tenantsRepository = tenantsRepository;
            _trucksRepository = trucksRepository;
            _appNotifier = appNotifier;
            _backgroundJobManager = backgroundJobManager;
        }

        private readonly IRepository<ShippingRequestBid, long> _shippingRequestBidsRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestsRepository;
        private readonly IRepository<Tenant> _tenantsRepository;
        private readonly IRepository<Truck, long> _trucksRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly BackgroundJobManager _backgroundJobManager;

        /// <summary>
        ///     This is for shipper to view Shipping Request bids in view-shipping-request page.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [RequiresFeature(AppFeatures.Shipper)]
        public virtual async Task<PagedResultDto<ShippingRequestBidDto>> GetAllShippingRequestBids(GetAllShippingRequestBidsInput input)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                IQueryable<ShippingRequestBid> filterShippingRequestsBids = _shippingRequestBidsRepository.GetAll()
                    .Include(x => x.Tenant)
                    .Where(e => e.ShippingRequestId == input.ShippingRequestId)
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
        }

        /// <summary>
        /// Shipper can cancel shipping request
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CancelBidShippingRequest(CancelBidShippingRequestInput input)
        {
            ShippingRequest bid = await _shippingRequestsRepository.GetAsync(input.ShippingRequestId);
            if (bid.ShippingRequestBidStatusId != TACHYONConsts.ShippingRequestStatusOnGoing)
            {
                ThrowShippingRequestIsNotOngoingError();
            }
            else
            {
                bid.ShippingRequestBidStatusId = TACHYONConsts.ShippingRequestStatusCanceled;
                bid.CloseBidDate = Clock.Now;
            }
        }


        public async Task<long> CreateOrEditShippingRequestBid(CreatOrEditShippingRequestBidDto input)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                ShippingRequest item = await _shippingRequestsRepository.GetAsync(input.ShippingRequestId);

                if (item.ShippingRequestBidStatusId != TACHYONConsts.ShippingRequestStatusOnGoing)
                {
                    ThrowShippingRequestIsNotOngoingError();
                }
            }

            if (input.Id == null)
            {
                return await Create(input);
            }

            return await Edit(input);
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
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                ShippingRequestBid bid = await _shippingRequestBidsRepository.GetAll()
                    .Include(x => x.ShippingRequestFk)
                    .FirstOrDefaultAsync(x => x.Id == shippingRequestBidId);
                if (bid == null)
                {
                    throw new UserFriendlyException(L("Bid Is not exist message"));
                }

                if (bid.ShippingRequestFk.ShippingRequestBidStatusId != TACHYONConsts.ShippingRequestStatusOnGoing)
                {
                    ThrowShippingRequestIsNotOngoingError();
                }

                bid.IsAccepted = true;

                //#540 notification to carrier told bid accepted
                await _appNotifier.AcceptShippingRequestBid(new UserIdentifier(bid.TenantId, bid.CreatorUserId.Value), bid.ShippingRequestId);

                //Reject the other bids of this shipping request by background job
                await _backgroundJobManager.EnqueueAsync<RejectOtherBidsJob, RejectOtherBidsJobArgs>
                    (new RejectOtherBidsJobArgs { AcceptedBidId = bid.Id, ShippingReuquestId = bid.ShippingRequestId });


                //update shippingRequest final price
                //todo review this to commission task team
                ShippingRequest shippingRequestItem = await _shippingRequestsRepository.FirstOrDefaultAsync(bid.ShippingRequestId);
                shippingRequestItem.Price = Convert.ToDecimal(bid.price);
                shippingRequestItem.Close();

            }
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
                    .Include(x => x.ShippingRequestFk)
                    .FirstOrDefaultAsync(x => x.Id == input.ShippingRequestBidId);
                if (bid == null)
                {
                    throw new UserFriendlyException(L("the bid is not exists message"));
                }

                //check if the bid is already canceled
                if (bid.IsCancled)
                {
                    throw new UserFriendlyException(L("bid is already canceled message"));
                }

                //Check if Shipping Request is not ongoing -- add cancel reason
                if (bid.ShippingRequestFk.ShippingRequestBidStatusId != TACHYONConsts.ShippingRequestStatusOnGoing)
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
        public virtual async Task<PagedResultDto<GetAllBidShippingRequestsForCarrierOutput>> GetAllBidShippingRequestsForCarrier(GetAllBidsShippingRequestForCarrierInput input)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                IQueryable<ShippingRequest> filterBidShippingRequests = _shippingRequestsRepository.GetAll()
                    .Include(x => x.ShippingRequestBidStatusFK)
                    .Include(x => x.TrucksTypeFk)
                    .Include(x => x.GoodCategoryFk)
                    .Include(x => x.GoodCategoryFk)
                    .Include(x => x.ShippingRequestBids)
                    .Include(x => x.RouteFk.OriginFacilityFk)
                    .ThenInclude(x=>x.CityFk)
                    .Include(x => x.RouteFk.DestinationFacilityFk)
                    .ThenInclude(x=>x.CityFk)
                    .Include(x => x.Tenant)
                    .Where(x => x.IsBid)
                    .WhereIf(input.TruckTypeId != null, x => x.TrucksTypeId == input.TruckTypeId)
                    .WhereIf(input.TransportType != null, x => x.TransportTypeId != null && x.TransportTypeId == input.TransportType)

                    //Filter
                    .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), x => x.ShippingRequestBids.Any(b => b.Tenant.Name.Contains(input.Filter)))

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


                IQueryable<ShippingRequest> pagedAndFilteredBidShippingRequests = filterBidShippingRequests
                    .OrderBy(input.Sorting ?? "id asc")
                    .PageBy(input);


                IEnumerable<GetAllBidShippingRequestsForCarrierOutput> shippingRequestBids = (await pagedAndFilteredBidShippingRequests.ToListAsync())
                    .Select(o => new GetAllBidShippingRequestsForCarrierOutput
                    {
                        ShippingRequestId = o.Id,
                        BidEndDate = o.BidEndDate,
                        BidStartDate = o.BidStartDate,
                        ShippingRequestBidStatusName = o.ShippingRequestBidStatusFK.DisplayName,
                        ShipperName = o.Tenant.Name,
                        TruckTypeDisplayName = o.TrucksTypeFk.DisplayName,
                        GoodCategoryName = o.GoodCategoryFk.DisplayName,
                        MyBidPrice = o.ShippingRequestBids.OrderByDescending(x => x.Id).FirstOrDefault()?.price,
                        MyBidId = o.ShippingRequestBids.FirstOrDefault()?.Id,
                        OriginalFacility=new GetFacilityForViewOutput()
                        {
                            Facility =ObjectMapper.Map<FacilityDto>(o.RouteFk.OriginFacilityFk),
                            CityDisplayName = o.RouteFk?.OriginFacilityFk?.CityFk.DisplayName,
                            FacilityName = o.RouteFk?.OriginFacilityFk?.Name
                        },
                        DestinationFacility =new GetFacilityForViewOutput()
                        {
                            Facility = ObjectMapper.Map<FacilityDto>(o.RouteFk.DestinationFacilityFk),
                            CityDisplayName = o.RouteFk?.DestinationFacilityFk?.CityFk.DisplayName,
                            FacilityName = o.RouteFk?.DestinationFacilityFk?.Name
                        } 
                    });

                int totalCount = await filterBidShippingRequests.CountAsync();

                return new PagedResultDto<GetAllBidShippingRequestsForCarrierOutput>(totalCount, shippingRequestBids.ToList());
            }
        }

        //#538
        [RequiresFeature(AppFeatures.Carrier)]
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids_Create)]
        private async Task<long> Create(CreatOrEditShippingRequestBidDto input)
        {
            var count = await _shippingRequestBidsRepository.CountAsync(x => x.ShippingRequestId == input.ShippingRequestId);

            if (count > 0)
            {
                throw new UserFriendlyException(L("You have already Bid to this shipping before message"));
            }

            ShippingRequestBid shippingRequestBid = ObjectMapper.Map<ShippingRequestBid>(input);
            if (AbpSession.TenantId != null)
            {
                shippingRequestBid.TenantId = (int)AbpSession.TenantId;
            }

            await _shippingRequestBidsRepository.InsertAndGetIdAsync(shippingRequestBid);

            await CurrentUnitOfWork.SaveChangesAsync();

            //notification to shipper when Carrier create new bid in his Shipping Request
            await _appNotifier.CreateBidRequest(
                new UserIdentifier(shippingRequestBid.ShippingRequestFk.TenantId, shippingRequestBid.ShippingRequestFk.CreatorUserId.Value),
                shippingRequestBid.Id);

            return shippingRequestBid.Id;
        }


        [RequiresFeature(AppFeatures.Carrier)]
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids_Edit)]
        private async Task<long> Edit(CreatOrEditShippingRequestBidDto input)
        {
            ShippingRequestBid item = await _shippingRequestBidsRepository.FirstOrDefaultAsync(input.Id.Value);
            ObjectMapper.Map(input, item);
            return item.Id;
        }

        private void ThrowShippingRequestIsNotOngoingError()
        {
            throw new UserFriendlyException(L("The Bid must be Ongoing message"));
        }
    }
}