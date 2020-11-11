using Abp.Application.Services.Dto;
using Abp.Linq.Extensions;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Shipping.ShippingRequestBids.Dtos;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Exporting;
using Abp.UI;
using Abp.Timing;
using Abp.Authorization;
using TACHYON.Authorization;
using Abp.Application.Features;
using TACHYON.Features;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System.Collections.Generic;
using Abp.Domain.Uow;
using NUglify.Helpers;
using Stripe;
using TACHYON.Notifications;
using Abp;
using System.Configuration;
using Microsoft.Extensions.Logging;

namespace TACHYON.Shipping.ShippingRequestBids
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids)]
    public class ShippingRequestBidsAppService : TACHYONAppServiceBase, IShippingRequestBidsAppService
    {
        private readonly IRepository<ShippingRequestBid, long> _shippingRequestBidsRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestsRepository;
        private readonly IAppNotifier _appNotifier;

        public ShippingRequestBidsAppService(IRepository<ShippingRequestBid, long> shippingRequestBidsRepository,
            IRepository<ShippingRequest, long> shippingRequestsRepository,
            IAppNotifier appNotifier)
        {
            _shippingRequestBidsRepository = shippingRequestBidsRepository;
            _shippingRequestsRepository = shippingRequestsRepository;
            _appNotifier = appNotifier;
        }

        //This is for shipper to view SR bids.
        [RequiresFeature(AppFeatures.Shipper)]
        public virtual async Task<PagedResultDto<GetShippingRequestBidsForViewDto>> GetAllBidsByShippingRequestIdPaging(GetAllShippingRequestBidsInput input)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                var filterShippingRequestsBids = _shippingRequestBidsRepository.GetAll()
                    .Where(e => e.ShippingRequestId == input.ShippingRequestId)
                    .Where(x => !x.IsCancled)
                    .WhereIf(input.MinPrice != null, e => e.price >= input.MinPrice)
                    .WhereIf(input.MaxPrice != null, e => e.price <= input.MaxPrice);


                var pagedAndFilteredShippingRequestsBids = filterShippingRequestsBids
                    .OrderBy(input.Sorting ?? "id asc")
                    .PageBy(input);

                var shippingRequestBids = from o in pagedAndFilteredShippingRequestsBids
                                          select new GetShippingRequestBidsForViewDto()
                                          {
                                              ShippingRequestBid = new ShippingRequestBidsDto
                                              {
                                                  Id = o.Id,
                                                  ShippingRequestId = o.ShippingRequestId,
                                                  IsAccepted = o.IsAccepted,
                                                  IsRejected=o.IsRejected,
                                                  price = o.price
                                              },
                                          };

                var totalCount = await filterShippingRequestsBids.CountAsync();

                return new PagedResultDto<GetShippingRequestBidsForViewDto>(totalCount, await shippingRequestBids.ToListAsync());

            }
        }

        public async Task CancelShippingRequestBid(StopShippingRequestBidInput input)
        {
            var bid = await _shippingRequestsRepository.FirstOrDefaultAsync(input.ShippingRequestId);
            if (bid.ShippingRequestBidStatusId != TACHYONConsts.ShippingRequestStatusOnGoing)
            {
                ThrowSRnotOngoingError();
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
                var item = await _shippingRequestsRepository.FirstOrDefaultAsync(x => x.Id == input.ShippingRequestId);
                if (item == null)
                {
                    throw new UserFriendlyException(L("Shipping request Is not Exists message"));
                }
                else
                {
                    if (item.ShippingRequestBidStatusId != TACHYONConsts.ShippingRequestStatusOnGoing)
                    {
                        ThrowSRnotOngoingError();
                        return 0;
                    }
                    else
                    {
                        if (input.Id == null)
                            return await Create(input);
                        else
                            return await Edit(input);
                    }
                }
            }
        }

        //#538
        [RequiresFeature(AppFeatures.Carrier)]
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids_Create)]
        protected virtual async Task<long> Create(CreatOrEditShippingRequestBidDto input)
        {
            var exist = CheckIfCarrierHasBidToSR(input.ShippingRequestId);

            if (exist.Result > 0)
            { 
                throw new UserFriendlyException(L("You have already Bid to this shipping before message"));
            }
            else
            {
                var shippingRequestBid = ObjectMapper.Map<ShippingRequestBid>(input);
                if (AbpSession.TenantId != null)
                {
                    shippingRequestBid.TenantId = (int)AbpSession.TenantId;

                }
                await _shippingRequestBidsRepository.InsertAsync(shippingRequestBid);

                //notification to shipper when Carrier create new bid in his SR
                await _appNotifier.CreateBidRequest(
                    new UserIdentifier(shippingRequestBid.ShippingRequestFk.TenantId, shippingRequestBid.ShippingRequestFk.CreatorUserId.Value),
                    shippingRequestBid.Id);

                return shippingRequestBid.Id;
            }
        }

        private async Task<int> CheckIfCarrierHasBidToSR(long ShippingRequestId)
        {
            return await _shippingRequestBidsRepository.CountAsync(x => x.TenantId == AbpSession.TenantId
               && x.ShippingRequestId == ShippingRequestId);
        }

        [RequiresFeature(AppFeatures.Carrier)]
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids_Edit)]
        protected async Task<long> Edit(CreatOrEditShippingRequestBidDto input)
        {
            var item = await _shippingRequestBidsRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, item);
            return item.Id;
        }

        //shipper accept carrier bid request #539
        [RequiresFeature(AppFeatures.Shipper)]
        public async Task AcceptBid(ShippingRequestBidInput input)
        {
            var bid = await _shippingRequestBidsRepository.FirstOrDefaultAsync(input.ShippingRequestBidId);
            if (bid != null)
            {
                if(bid.ShippingRequestFk.ShippingRequestBidStatusId != TACHYONConsts.ShippingRequestStatusOnGoing)
                {
                    ThrowSRnotOngoingError();
                }
                bid.IsAccepted = true;

                //#540 notification to carrier told bid accepted

                await _appNotifier.AcceptShippingRequestBid(new UserIdentifier(bid.TenantId, bid.CreatorUserId.Value), bid.ShippingRequestId);

                //Reject the other bids of this shipping request
                var otherBids = _shippingRequestBidsRepository.GetAll()
                    .Where(x => x.ShippingRequestId == bid.ShippingRequestId)
                    .Where(x => x.Id != bid.Id);
                foreach (var item in otherBids)
                {
                    item.IsRejected = true;
                }

                //update shippingRequest final price
                var shippingRequestItem = _shippingRequestsRepository.FirstOrDefault(bid.ShippingRequestId);
                shippingRequestItem.Price = Convert.ToDecimal(bid.price);
                shippingRequestItem.Close();

            }
            else
            {
                throw new UserFriendlyException(L("Bid Is not exist message"));
            }
        }

        protected void ThrowSRnotOngoingError()
        {
            throw new UserFriendlyException(L("The Bid must be Ongoing message"));
        }
        //#541
        [RequiresFeature(AppFeatures.Carrier)]
        public async Task<List<ViewCarrierBidsOutput>> ViewAllCarrierBids()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                var shippingRequests = await _shippingRequestBidsRepository.GetAll()
                    .Include(x => x.ShippingRequestFk)
                    .Where(x => x.TenantId == AbpSession.TenantId)
                    //.Select(x => x.ShippingRequestFk)
                    .ToListAsync();


                return ObjectMapper.Map<List<ViewCarrierBidsOutput>>(shippingRequests);
            }
        }

        [RequiresFeature(AppFeatures.Carrier)]
        public async Task CancelBidRequest(ShippingRequestBidInput input)
        {
            var bid = await _shippingRequestBidsRepository.FirstOrDefaultAsync(input.ShippingRequestBidId);
            if (bid.IsCancled != true)
            {
                throw new UserFriendlyException(L("bid is already canceled message"));
            }


            bid.IsCancled = true;
            bid.CanceledDate = Clock.Now;

            //notification to shipper when Carrier cancel his bid in his SR
            await _appNotifier.CancelBidRequest(
                new UserIdentifier(bid.ShippingRequestFk.TenantId, bid.ShippingRequestFk.CreatorUserId.Value), 
                bid.ShippingRequestId,
                bid.Id);
        }

        //#542 Shipper can view all his bids requests with current status
        [RequiresFeature(AppFeatures.Shipper)]
        public virtual async Task<PagedResultDto<ViewShipperBidsReqDetailsOutputDto>> GetShipperbidsRequestDetailsForView(PagedAndSortedResultRequestDto input)
        {
            return await GetAllBids(input);
        }

        //#537 get All Shippers Shipping Requests to view for carrier
        [RequiresFeature(AppFeatures.Carrier)]
        public virtual async Task<PagedResultDto<ViewShipperBidsReqDetailsOutputDto>> GetAllbidsRequestDetailsForView(PagedAndSortedResultRequestDto input)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                return await GetAllBids(input);
            }
        }

        protected async Task<PagedResultDto<ViewShipperBidsReqDetailsOutputDto>> GetAllBids(PagedAndSortedResultRequestDto input)
        {
            var filterShippingRequestsBids = _shippingRequestsRepository.GetAll()
                    .Include(x => x.ShippingRequestBidStatusFK)
                    .Where(x => x.IsBid == true)
                   .Where(x => x.ShippingRequestBidStatusId != TACHYONConsts.ShippingRequestStatusCanceled);

            var pagedAndFilteredShippingRequestsBids = filterShippingRequestsBids
            .OrderBy(input.Sorting ?? "id asc")
            .PageBy(input);

            var shippingRequestBids = from o in pagedAndFilteredShippingRequestsBids
                                      select new ViewShipperBidsReqDetailsOutputDto()
                                      {
                                          viewShipperBidsReqDetailsOutput = new ViewShipperBidsReqDetailsOutput
                                          {
                                              Id = o.Id,
                                              EndBidDate = o.BidEndDate,
                                              IsOngoingBid = o.ShippingRequestBidStatusId == TACHYONConsts.ShippingRequestStatusOnGoing ? true : false,
                                              ShippingRequestBidStatusName = o.ShippingRequestBidStatusFK.DisplayName,
                                              ShipperName = o.CarrierTenantFk.Edition.DisplayName,
                                              StartBidDate = o.BidStartDate,
                                              TruckTypeDisplayName = o.TransportSubtypeFk.DisplayName,
                                              GoodCategoryName = o.GoodCategoryFk.DisplayName,
                                              BidsNo = o.ShippingRequestBids.Count(),
                                              LastBidPrice = o.ShippingRequestBids.Where(x => x.IsCancled == false).OrderByDescending(x => x.Id).FirstOrDefault().price,
                                              FirstBidId = o.ShippingRequestBids.FirstOrDefault().Id
                                          },
                                      };

            var totalCount = await filterShippingRequestsBids.CountAsync();

            return new PagedResultDto<ViewShipperBidsReqDetailsOutputDto>(totalCount, await shippingRequestBids.ToListAsync());

        }
    }
}
