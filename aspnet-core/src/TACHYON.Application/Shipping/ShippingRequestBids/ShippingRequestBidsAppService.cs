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

namespace TACHYON.Shipping.ShippingRequestBids
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids)]
    public class ShippingRequestBidsAppService : TACHYONAppServiceBase, IShippingRequestBidsAppService
    {
        private readonly IRepository<ShippingRequestBid, long> _shippingRequestBidsRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestsRepository;

        public ShippingRequestBidsAppService(IRepository<ShippingRequestBid, long> shippingRequestBidsRepository,
            IRepository<ShippingRequest, long> shippingRequestsRepository)
        {
            _shippingRequestBidsRepository = shippingRequestBidsRepository;
            _shippingRequestsRepository = shippingRequestsRepository;
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
            if (bid.IsClosedBid == true)
            {
                throw new UserFriendlyException(L("The Bid is already Closed message"));
            }
            else
            {
                bid.IsClosedBid = true;
                bid.CloseBidDate = Clock.Now;
            }
        }

       // [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids)]
        public async Task CreateOrEditShippingRequestBid(CreatOrEditShippingRequestBidDto input)
        {
            if (input.Id == null)
                await Create(input);
            else
                Edit(input);
        }

            //#538
        [RequiresFeature(AppFeatures.Carrier)]
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids_Create)]
        protected virtual async Task  Create(CreatOrEditShippingRequestBidDto input)
        {
            var shippingRequestBid = ObjectMapper.Map<ShippingRequestBid>(input);
            if (AbpSession.TenantId != null)
            {
                shippingRequestBid.TenantId = (int)AbpSession.TenantId;

            }
            await _shippingRequestBidsRepository.InsertAsync(shippingRequestBid);

        }

        [RequiresFeature(AppFeatures.Carrier)]
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids_Edit)]
        protected void Edit(CreatOrEditShippingRequestBidDto input)
        {
            var item = _shippingRequestBidsRepository.FirstOrDefaultAsync((long)input.Id);

             ObjectMapper.Map(input, item);
        }

        //shipper accept carrier bid request #539
        [RequiresFeature(AppFeatures.Shipper)]
      //  [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids)]
        public async Task AcceptBid(ShippingRequestBidInput input)
        {
            var bid =await _shippingRequestBidsRepository.FirstOrDefaultAsync(input.ShippingRequestBidId);
            if (bid != null)
            {
                bid.IsAccepted = true;
                //Reject the other bids of this shipping request
                var otherBids = _shippingRequestBidsRepository.GetAll().Where(x => x.ShippingRequestId == bid.ShippingRequestId && x.Id != bid.Id);
                foreach(var item in otherBids)
                {
                    item.IsAccepted = false;
                }
            }
            else
            {
                throw new UserFriendlyException(L("Bid Is not exist message"));
            }
        }

        //#541
        [RequiresFeature(AppFeatures.Carrier)]
        public async Task<List<ViewCarrierBidsOutput>> ViewAllCarrierOngoingBids()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                var bids = await _shippingRequestsRepository.GetAll()
                .Where(x=> x.ShippingRequestBids != null &&
                x.ShippingRequestBids.Any(y=>y.TenantId==AbpSession.TenantId))
                .ToListAsync();
                return ObjectMapper.Map<List<ViewCarrierBidsOutput>>(bids);
            }
        }

        [RequiresFeature(AppFeatures.Carrier)]
        public async Task CancleBidRequest(ShippingRequestBidInput input)
        {
            var bid =await _shippingRequestBidsRepository.FirstOrDefaultAsync(input.ShippingRequestBidId);
            if (bid.IsCancled != true)
            {
                throw new UserFriendlyException(L("The bid is already cancled message"));
            }
            bid.IsCancled = true;
            bid.CanceledDate = Clock.Now;
        }

        //#537
        public async Task<PagedResultDto<ViewShipperBidsReqDetailsOutputDto>> ViewShipperBidsReqDetails(PagedAndSortedResultRequestDto input)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                var filterShippingRequestsBids = _shippingRequestsRepository.GetAll()
                   .Where(x => x.isCancledBid!=true);

                var pagedAndFilteredShippingRequestsBids = filterShippingRequestsBids
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

                var shippingRequestBids = from o in pagedAndFilteredShippingRequestsBids
                                          select new ViewShipperBidsReqDetailsOutputDto()
                                          {
                                              viewShipperBidsReqDetailsOutput = new ViewShipperBidsReqDetailsOutput
                                              {
                                                  Id = o.Id,
                                                  EndBidDate = o.BidEndDate ,
                                                  IsOngoingBid = o.IsClosedBid !=true ? true: false,
                                                  ShipperName=o.CarrierTenantFk.Edition.DisplayName
                                              },
                                          };

                var totalCount = await filterShippingRequestsBids.CountAsync();

                return new PagedResultDto<ViewShipperBidsReqDetailsOutputDto>(totalCount, await shippingRequestBids.ToListAsync());

            }
        }
    }
}
