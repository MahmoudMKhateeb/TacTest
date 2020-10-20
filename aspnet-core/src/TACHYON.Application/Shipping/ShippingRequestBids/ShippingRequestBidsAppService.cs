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

namespace TACHYON.Shipping.ShippingRequestBids
{
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

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids)]
        public async Task CreateOrEditShippingRequestBid(CreatOrEditShippingRequestBidDto input)
        {
            if (input.Id == null)
                await Create(input);
            else
                Edit(input);
        }

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
    }
}
