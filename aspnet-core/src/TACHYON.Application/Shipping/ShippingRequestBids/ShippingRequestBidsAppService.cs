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

        //#542 This is for carrier
        public virtual async Task<PagedResultDto<GetShippingRequestBidsForViewDto>> GetAllBids(GetAllShippingRequestBidsInput input)
        {
            var filterShippingRequestsBids = _shippingRequestBidsRepository.GetAll()
                //.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false)
                .WhereIf(input.MinPrice != null, e => e.price >= input.MinPrice)
                .WhereIf(input.MaxPrice != null, e => e.price <= input.MaxPrice)
                .WhereIf(input.ShippingRequestId != null, e => e.ShippingRequestId == input.ShippingRequestId)
                .Where(x => !x.IsCancled);

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
                                              IsCancled = o.IsCancled,
                                              price = o.price
                                          },
                                      };

            var totalCount = await filterShippingRequestsBids.CountAsync();

            return new PagedResultDto<GetShippingRequestBidsForViewDto>(totalCount, await shippingRequestBids.ToListAsync());


        }

        public async Task CloseShippingRequestBid(StopShippingRequestBidInput input)
        {
            var bid = await _shippingRequestsRepository.FirstOrDefaultAsync(input.ShippingRequestId);
            if (bid.ShippingRequestStatusId == TACHYONConsts.ShippingRequestStatusClosed)
            {
                throw new UserFriendlyException(L("The Bid is already Closed message"));
            }
            if (bid.ShippingRequestStatusId != TACHYONConsts.ShippingRequestStatusOnGoing)
            {
                throw new UserFriendlyException(L("The Bid must be Ongoing message"));
            }
            else
            {
                bid.ShippingRequestStatusId = TACHYONConsts.ShippingRequestStatusClosed;
                bid.CloseBidDate = Clock.Now;
            }
        }


        public async Task<long> CreateOrEditShippingRequestBid(CreatOrEditShippingRequestBidDto input)
        {
            if (input.Id == null)
                return await Create(input);
            else
                return await Edit(input);
        }

        //#538
        [RequiresFeature(AppFeatures.Carrier)]
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids_Create)]
        protected virtual async Task<long> Create(CreatOrEditShippingRequestBidDto input)
        {
            var exist = CheckIfCarrierHasBidToSR(input.ShippingRequestId);

            if (exist.Result == 0)
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
            var item =await _shippingRequestBidsRepository.FirstOrDefaultAsync((long)input.Id);

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

        //#541
        [RequiresFeature(AppFeatures.Carrier)]
        public async Task<List<ViewCarrierBidsOutput>> ViewAllCarrierBids()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                var shippingRequests = await _shippingRequestBidsRepository.GetAll()
                    .Include(x => x.ShippingRequestFk)
                    .Where(x => x.TenantId == AbpSession.TenantId)
                    .Select(x => x.ShippingRequestFk).
                    ToListAsync();


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
        }

        //#537
        public virtual async Task<PagedResultDto<ViewShipperBidsReqDetailsOutputDto>> GetShipperbidsRequestDetailsForView(PagedAndSortedResultRequestDto input)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                var filterShippingRequestsBids = _shippingRequestsRepository.GetAll()
                    .Include(x => x.ShippingRequestStatusFk)
                   .Where(x => x.ShippingRequestStatusId != TACHYONConsts.ShippingRequestStatusCanceled);

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
                                                  IsOngoingBid = o.ShippingRequestStatusId == TACHYONConsts.ShippingRequestStatusOnGoing ? true : false,
                                                  ShippingRequestBidStatusName = o.ShippingRequestStatusFk.DisplayName,
                                                  ShipperName = o.CarrierTenantFk.Edition.DisplayName,
                                                  StartBidDate = o.BidStartDate,
                                                  TruckTypeDisplayName=o.TransportSubtypeFk.DisplayName,
                                                  GoodCategoryName=o.GoodCategoryFk.DisplayName,
                                                  BidsNo=o.ShippingRequestBids.Count(),
                                                  LastBidPrice=o.ShippingRequestBids.Where(x=>x.IsCancled==false).OrderByDescending(x=>x.Id).FirstOrDefault().price
                                              },
                                          };

                var totalCount = await filterShippingRequestsBids.CountAsync();

                return new PagedResultDto<ViewShipperBidsReqDetailsOutputDto>(totalCount, await shippingRequestBids.ToListAsync());

            }
        }
    }
}
